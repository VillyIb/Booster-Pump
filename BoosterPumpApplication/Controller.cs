using System;
using System.Linq;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using BoosterPumpConfiguration;
using BoosterPumpLibrary.Logger;

using Modules;
using System.Diagnostics.CodeAnalysis;
using BoosterPumpLibrary.Contracts;
using NCD_API_SerialConverter;
using NCD_API_SerialConverter.NcdApiProtocol.SerialConverterCommands;

namespace BoosterPumpApplication
{
    [ExcludeFromCodeCoverage]
    public class Controller : IController
    {
        public MeasurementSettings MeasurementSettings { get; }

        public IServiceProvider ServiceProvider { get; }

        public ISerialConverter SerialConverter { get; }

        public ControllerSettings ControllerSettings { get; }

        public Controller(
            IServiceProvider serviceProvider,
            IOptionsSnapshot<MeasurementSettings> measurementSettings,
            IOptionsSnapshot<ControllerSettings> controllerSettings,
            ISerialConverter serialConverter
        )
        {
            MeasurementSettings = measurementSettings.Value;
            ServiceProvider = serviceProvider;
            SerialConverter = serialConverter;
            ControllerSettings = controllerSettings.Value;
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

        public void Init()
        {
            Console.WriteLine("+Controller.ExecuteAsync");
            Stopwatch = new Stopwatch();

            DisplayModule = ServiceProvider.GetRequiredService<As1115Module>();
            DisplayModule.Init();

            ManifoldPressureDifference = ServiceProvider.GetRequiredService<AMS5812_0150_D_B_Module>();

            FlowNorthWest = ServiceProvider.GetRequiredService<AMS5812_0150_D_B_Module>();

            FlowSouthEast = ServiceProvider.GetRequiredService<AMS5812_0150_D_B_Module>();

            SystemPressure = ServiceProvider.GetRequiredService<AMS5812_0300_A_PressureModule>();

            BarometerModule1 = ServiceProvider.GetRequiredService<LPS25HB_BarometerModule>();
            BarometerModule2 = ServiceProvider.GetRequiredService<LPS25HB_BarometerModule>();
            BarometerModule2.SetAddressIncrement(1);

            Multiplexer = ServiceProvider.GetRequiredService<TCA9546MultiplexerModule>();

            SpeedController = ServiceProvider.GetRequiredService<MCP4725_4_20mA_CurrentTransmitterV2>();

            FlowNorthWestStack = new System.Collections.Generic.Queue<double>();
            FlowSouthEastStack = new System.Collections.Generic.Queue<double>();

            LogWriter = ServiceProvider.GetRequiredService<IBufferedLogWriter>();

            Speed2 = 0.50f;
            SpeedCurrent = 0.01f;
        }

        public void Execute(IBufferedLogWriter logger)
        {
            try
            {

                do
                {
                    try
                    {
                        Stopwatch.Reset();
                        Stopwatch.Start();

                        {
                            var ncdCommand = new ConverterScan();
                            var dataFromDevice = ((SerialConverter)SerialConverter).Execute(ncdCommand);
                            if (!(dataFromDevice.IsValid))
                            {
                                throw new ApplicationException(dataFromDevice.ToString());
                            }
                        }

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

                            var flowNorthWestvalue = Math.Log10(Math.Max(0.1, flowNorthWestAverage) + 1);
                            var flowSouthEastValue = Math.Log10(Math.Max(0.1, flowSouthEastAverage) + 1);

                            var controllingFlow = flowNorthWestvalue * ControllerSettings.WestGradient + flowSouthEastValue * ControllerSettings.EastGradient;
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

                        var now = DateTime.UtcNow;
                        now = new DateTime(
                            now.Ticks - now.Ticks % (TimeSpan.TicksPerSecond / 10),
                            now.Kind
                        );

                        var fill = "".PadLeft(80, ' ');

                        // Headline: "Timestamp;Second of day;Manifold-P;NW-F;SE-F;Sys-P;Speed;DSpeed;Bar1-P;Bar2-P;Bar1-T;Bar2-T;Manifold-T;NW-T;SE-T;Sys-T;Com-Grad;Com-Intc"

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

                        var bufferline = new BufferLineMeasurement(now, payload);

                        SpeedCurrent = Speed2;

                        Console.Write($"\r{fill}\r{bufferline.LogText}\r");
                        LogWriter.Add(bufferline);

                        Stopwatch.Stop();

                    }
                    catch (ApplicationException)
                    {
                        var ncdCommand = new ConverterTest2Way();
                        var result = ((SerialConverter)SerialConverter).Execute(ncdCommand);

                        Console.WriteLine($"Reset serial converter, {result}");
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
            }

        }

        public async Task ExecuteAsync(CancellationToken cancellationToken, IBufferedLogWriter logger)
        {
            try
            {
                Console.WriteLine("+Controller.ExecuteAsync");

                if (null == Stopwatch) { Init(); }

                do
                {
                    try
                    {
                        Stopwatch.Reset();
                        Stopwatch.Start();

                        {
                            var ncdCommand = new ConverterScan();
                            var dataFromDevice = ((SerialConverter)SerialConverter).Execute(ncdCommand);
                            if (!(dataFromDevice.IsValid))
                            {
                                throw new ApplicationException(dataFromDevice.ToString());
                            }
                        }

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

                            var flowNorthWestvalue = Math.Log10(Math.Max(0.1, flowNorthWestAverage) + 1);
                            var flowSouthEastValue = Math.Log10(Math.Max(0.1, flowSouthEastAverage) + 1);

                            var controllingFlow = flowNorthWestvalue * ControllerSettings.WestGradient + flowSouthEastValue * ControllerSettings.EastGradient;
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

                        var now = DateTime.UtcNow;
                        now = new DateTime(
                            now.Ticks - now.Ticks % (TimeSpan.TicksPerSecond / 10),
                            now.Kind
                        );

                        var fill = "".PadLeft(80, ' ');

                        // Headline: "Timestamp;Second of day;Manifold-P;NW-F;SE-F;Sys-P;Speed;DSpeed;Bar1-P;Bar2-P;Bar1-T;Bar2-T;Manifold-T;NW-T;SE-T;Sys-T;Com-Grad;Com-Intc"

                        var payload = new []
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

                        var bufferline = new BufferLineMeasurement(now, payload);

                        SpeedCurrent = Speed2;

                        Console.Write($"\r{fill}\r{bufferline.LogText}\r");
                        LogWriter.Add(bufferline);

                        Stopwatch.Stop();
                        var duration = (int)Stopwatch.ElapsedMilliseconds;
                        if (duration < MeasurementSettings.RoundTripTime)
                        {
                            await Task.Delay(MeasurementSettings.RoundTripTime - duration);
                        }
                    }
                    catch (ApplicationException)
                    {
                        var ncdCommand = new ConverterTest2Way();
                        var result = ((SerialConverter)SerialConverter).Execute(ncdCommand);

                        Console.WriteLine($"Reset serial converter, {result}");
                    }
                } while (!cancellationToken.IsCancellationRequested);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
            }
        }
    }
}
