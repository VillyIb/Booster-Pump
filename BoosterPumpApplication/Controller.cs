using System;
using System.Linq;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;

using Modules;
using eu.iamia.Util;
using BoosterPumpLibrary.Logger;
using BoosterPumpConfiguration;
using eu.iamia.NCD.API;
using eu.iamia.NCD.API.Contract;
using eu.iamia.NCD.Serial.Contract;

namespace BoosterPumpApplication
{
    [ExcludeFromCodeCoverage]
    public class Controller : IController
    {
        private readonly IBridge ApiToSerialBridge;
        public MeasurementSettings MeasurementSettings { get; }

        public IGateway Gateway { get; }

        public ControllerSettings ControllerSettings { get; }

        private DeviceFactory DeviceFactory { get; }

        public Controller(
            IOptions<MeasurementSettings> measurementSettings,
            IOptions<ControllerSettings> controllerSettings,
            IGateway gateway,
            IBridge apiToSerialBridge
        )
        {
            ApiToSerialBridge = apiToSerialBridge;
            MeasurementSettings = measurementSettings.Value;
            Gateway = gateway;
            ControllerSettings = controllerSettings.Value;
            DeviceFactory = new DeviceFactory();

        }

        private Stopwatch Stopwatch;
        private As1115Module DisplayModule;
        private AMS5812_0150_D_B_Module ManifoldPressureDifference;
        private AMS5812_0150_D_B_Module FlowNorthWest;
        private AMS5812_0150_D_B_Module FlowSouthEast;
        private AMS5812_0300_A_PressureModule SystemPressure;
        private LPS25HB_BarometerModule BarometerModule1;
        private LPS25HB_BarometerModule BarometerModule2;
        private TCA9546MultiplexerModule Multiplexer;
        private MCP4725_4_20mA_CurrentTransmitterV2 SpeedController;
        private System.Collections.Generic.Queue<double> FlowNorthWestStack;
        private System.Collections.Generic.Queue<double> FlowSouthEastStack;
        private IBufferedLogWriter LogWriter;
        private float Speed2;
        private float SpeedCurrent;

        public void Init(IServiceScope scope)
        {
            Console.WriteLine("+Controller.ExecuteAsync");
            Stopwatch = new Stopwatch();

            DisplayModule = scope.ServiceProvider.GetRequiredService<As1115Module>();
            DisplayModule.Init();

            ManifoldPressureDifference = scope.ServiceProvider.GetRequiredService<AMS5812_0150_D_B_Module>();

            FlowNorthWest = scope.ServiceProvider.GetRequiredService<AMS5812_0150_D_B_Module>();

            FlowSouthEast = scope.ServiceProvider.GetRequiredService<AMS5812_0150_D_B_Module>();

            SystemPressure = scope.ServiceProvider.GetRequiredService<AMS5812_0300_A_PressureModule>();

            BarometerModule1 = scope.ServiceProvider.GetRequiredService<LPS25HB_BarometerModule>();
            BarometerModule2 = scope.ServiceProvider.GetRequiredService<LPS25HB_BarometerModule>();
            BarometerModule2.SetAddressIncrement(1);

            Multiplexer = scope.ServiceProvider.GetRequiredService<TCA9546MultiplexerModule>();

            SpeedController = scope.ServiceProvider.GetRequiredService<MCP4725_4_20mA_CurrentTransmitterV2>();

            FlowNorthWestStack = new System.Collections.Generic.Queue<double>();
            FlowSouthEastStack = new System.Collections.Generic.Queue<double>();

            LogWriter = scope.ServiceProvider.GetRequiredService<IBufferedLogWriter>();

            Speed2 = 0.50f;
            SpeedCurrent = 0.01f;
        }

        private void CheckSerialConverter()
        {
            var command = new CommandControllerControllerBusSCan();
            var dataFromDevice = ApiToSerialBridge.Execute(command);

            if (!dataFromDevice.IsValid)
            {
                throw new ApplicationException(dataFromDevice.ToString());
            }
        }

        public void Execute(IBufferedLogWriter logger)
        {
            try
            {
                try
                {
                    Stopwatch.Reset();
                    Stopwatch.Start();

                    CheckSerialConverter();

                    //barometerModule1.ReadDevice();
                    //barometerModule2.ReadDevice();

                    Multiplexer.SelectOpenChannels(MultiplexerChannels.Channel0);
                    {
                        ManifoldPressureDifference.ReadFromDevice();
                    }

                    Multiplexer.SelectOpenChannels(MultiplexerChannels.Channel1);
                    {
                        FlowNorthWest.ReadFromDevice();
                        FlowNorthWestStack.Enqueue(FlowNorthWest.Pressure + MeasurementSettings.FlowNorthWestCorrection);
                    }

                    Multiplexer.SelectOpenChannels(MultiplexerChannels.Channel2);
                    {
                        FlowSouthEast.ReadFromDevice();
                        FlowSouthEastStack.Enqueue(FlowSouthEast.Pressure + MeasurementSettings.FlowSouthEastCorrection);
                    }

                    Multiplexer.SelectOpenChannels(MultiplexerChannels.Channel3);
                    {
                        SystemPressure.ReadFromDevice();
                    }

                    Multiplexer.SelectOpenChannels(MultiplexerChannels.None);

                    if (FlowNorthWestStack.Count >= ControllerSettings.AverageSize)
                    {
                        // TODO remove spikes

                        var flowNorthWestAverage = FlowNorthWestStack.Average();
                        FlowNorthWestStack.Dequeue();
                        var flowSouthEastAverage = FlowSouthEastStack.Average();
                        FlowSouthEastStack.Dequeue();

                        var flowNorthWestValue = Math.Log10(Math.Max(0.1, flowNorthWestAverage) + 1);
                        var flowSouthEastValue = Math.Log10(Math.Max(0.1, flowSouthEastAverage) + 1);

                        var controllingFlow = flowNorthWestValue * ControllerSettings.WestGradient + flowSouthEastValue * ControllerSettings.EastGradient;
                        var speed1 = controllingFlow * ControllerSettings.CommonGradient + ControllerSettings.CommonIntercept;

                        Speed2 = (float)Math.Min(0.999, Math.Max(ControllerSettings.MinSpeedPct, speed1));
                    }

                    if (Math.Abs(SpeedCurrent - Speed2) > 0.0005f)
                    {
                        Multiplexer.SelectOpenChannels(MultiplexerChannels.Channel2);
                        {
                            DisplayModule.SetBcdValue(Speed2 * 100.0f);
                        }

                        Multiplexer.SelectOpenChannels(MultiplexerChannels.Channel3);
                        {
                            SpeedController.SetSpeed(Speed2);
                        }

                        Multiplexer.SelectOpenChannels(MultiplexerChannels.None);
                    }

                    var now = SystemDateTime.UtcNow;
                    now = new DateTime(
                        now.Ticks - now.Ticks % (TimeSpan.TicksPerSecond / 10),
                        now.Kind
                    );

                    var fill = "".PadLeft(80, ' ');

                    // Headline: "Timestamp;Second of day;Manifold-P;NW-F;SE-F;Sys-P;Speed;DSpeed;Bar1-P;Bar2-P;Bar1-T;Bar2-T;Manifold-T;NW-T;SE-T;Sys-T;Com-Grad;Com-IntC"

                    var payload = new[]
                    {
                            ManifoldPressureDifference.Pressure + MeasurementSettings.ManifoldPressureCorrection,
                            FlowNorthWest.Pressure + MeasurementSettings.FlowNorthWestCorrection,
                            FlowSouthEast.Pressure + MeasurementSettings.FlowSouthEastCorrection,
                            SystemPressure.Pressure + MeasurementSettings.SystemPressureCorrection
                            , Speed2 * 100.0f,
                            (SpeedCurrent - Speed2) * 100.0f, (float) BarometerModule1.AirPressure,
                            (float) BarometerModule2.AirPressure, (float) BarometerModule2.Temperature,
                            (float) BarometerModule1.Temperature, ManifoldPressureDifference.Temperature,
                            FlowNorthWest.Temperature, SystemPressure.Temperature, SystemPressure.Temperature,
                            ControllerSettings.CommonGradient, ControllerSettings.CommonIntercept, 0.0f
                        };

                    var bufferLine = new BufferLineMeasurement(now, payload);

                    SpeedCurrent = Speed2;

                    Console.Write($"\r{fill}\r{bufferLine.LogText}\r");
                    LogWriter.Add(bufferLine);

                    Stopwatch.Stop();

                }
                catch (ApplicationException)
                {
                    var command = new CommandControllerControllerTest2WayCommunication();
                    
                    var dataFromDevice = ApiToSerialBridge.Execute(command);

                    Console.WriteLine($"Reset serial converter, {dataFromDevice}");
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
                throw;
            }
        }
    }
}
