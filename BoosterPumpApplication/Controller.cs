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
using System.Text;
using System.Globalization;
using System.Diagnostics.CodeAnalysis;
using BoosterPumpLibrary.Contracts;
using NCD_API_SerialConverter;
using NCD_API_SerialConverter.NcdApiProtocol.SerialConverterCommands;

namespace BoosterPumpApplication
{
    [ExcludeFromCodeCoverage]
    public class Controller : IController
    {
        private static CultureInfo CultureInfo => CultureInfo.GetCultureInfo("da-DK"); //  da-DK

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

        public async Task ExecuteAsync(CancellationToken cancellationToken, IBufferedLogWriter logger)
        {
            try
            {
                Console.WriteLine("+Controller.ExecuteAsync");
                var stopwatch = new Stopwatch();

                var displayModule = ServiceProvider.GetRequiredService<As1115Module>();
                displayModule.Init();

                var manifoldPressureDifference = ServiceProvider.GetRequiredService<AMS5812_0150_D_B_Module>();

                var flowNorthWest = ServiceProvider.GetRequiredService<AMS5812_0150_D_B_Module>();

                var flowSouthEast = ServiceProvider.GetRequiredService<AMS5812_0150_D_B_Module>();

                var systemPressure = ServiceProvider.GetRequiredService<AMS5812_0300_A_PressureModule>();

                var barometerModule1 = ServiceProvider.GetRequiredService<LPS25HB_BarometerModule>();
                var barometerModule2 = ServiceProvider.GetRequiredService<LPS25HB_BarometerModule>();
                barometerModule2.SetAddressIncrement(1);

                var multiplexer = ServiceProvider.GetRequiredService<TCA9546MultiplexerModule>();

                var speedController = ServiceProvider.GetRequiredService<MCP4725_4_20mA_CurrentTransmitterV2>();

                var flowNorthWestStack = new System.Collections.Generic.Queue<double>();
                var flowSouthEastStack = new System.Collections.Generic.Queue<double>();

                var logWriter = ServiceProvider.GetRequiredService<IBufferedLogWriter>();



                float speed2 = 0.50f;
                float speedCurrent = 0.01f;

                do
                {
                    try
                    {
                        stopwatch.Reset();
                        stopwatch.Start();

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

                        multiplexer.SelectOpenChannels(MultiplexerChannels.Channel0);
                        {
                            manifoldPressureDifference.ReadFromDevice();
                        }

                        multiplexer.SelectOpenChannels(MultiplexerChannels.Channel1);
                        {
                            flowNorthWest.ReadFromDevice();
                            flowNorthWestStack.Enqueue(flowNorthWest.Pressure + MeasurementSettings.FlowNorthWestCorrection);
                        }

                        multiplexer.SelectOpenChannels(MultiplexerChannels.Channel2);
                        {
                            flowSouthEast.ReadFromDevice();
                            flowSouthEastStack.Enqueue(flowSouthEast.Pressure + MeasurementSettings.FlowSouthEastCorrection);
                        }

                        multiplexer.SelectOpenChannels(MultiplexerChannels.Channel3);
                        {
                            systemPressure.ReadFromDevice();
                        }

                        multiplexer.SelectOpenChannels(MultiplexerChannels.None);

                        if (flowNorthWestStack.Count >= ControllerSettings.AverageSize)
                        {
                            // TODO remove spikes

                            var flowNorthWestAverage = flowNorthWestStack.Average();
                            flowNorthWestStack.Dequeue();
                            var flowSouthEastAverage = flowSouthEastStack.Average();
                            flowSouthEastStack.Dequeue();

                            var flowNorthWestvalue = Math.Log10(Math.Max(0.1, flowNorthWestAverage) + 1) ;
                            var flowSouthEastValue = Math.Log10(Math.Max(0.1, flowSouthEastAverage) + 1);

                            var controllingFlow = flowNorthWestvalue * ControllerSettings.WestGradient + flowSouthEastValue * ControllerSettings.EastGradient;
                            var speed1 = controllingFlow * ControllerSettings.CommonGradient + ControllerSettings.CommonIntercept;

                            speed2 = (float)Math.Min(0.999, Math.Max(ControllerSettings.MinSpeedPct, speed1));
                        }

                        if (Math.Abs(speedCurrent - speed2) > 0.0005f)
                        {
                            multiplexer.SelectOpenChannels(MultiplexerChannels.Channel2);
                            {
                                displayModule.SetBcdValue(speed2 * 100.0f);
                            }

                            multiplexer.SelectOpenChannels(MultiplexerChannels.Channel3);
                            {
                                speedController.SetSpeed(speed2);
                            }

                            multiplexer.SelectOpenChannels(MultiplexerChannels.None);
                        }

                        var now = DateTime.UtcNow;
                        now = new DateTime(
                            now.Ticks - now.Ticks % (TimeSpan.TicksPerSecond / 10),
                            now.Kind
                        );

                        var fill = "".PadLeft(80, ' ');
                       
                        // Headline: "Timestamp;Second of day;Manifold-P;NW-F;SE-F;Sys-P;Speed;DSpeed;Bar1-P;Bar2-P;Bar1-T;Bar2-T;Manifold-T;NW-T;SE-T;Sys-T;Com-Grad;Com-Intc"

                        var payload = new float[]
                        {
                            manifoldPressureDifference.Pressure + MeasurementSettings.ManifoldPressureCorrection,
                            flowNorthWest.Pressure + MeasurementSettings.FlowNorthWestCorrection,
                            flowSouthEast.Pressure + MeasurementSettings.FlowSouthEastCorrection,
                            systemPressure.Pressure + MeasurementSettings.SystemPressureCorrection
                            , speed2 * 100.0f,
                            (speedCurrent - speed2) * 100.0f, (float) barometerModule1.AirPressure,
                            (float) barometerModule2.AirPressure, (float) barometerModule2.Temperature,
                            (float) barometerModule1.Temperature, manifoldPressureDifference.Temperature,
                            flowNorthWest.Temperature, systemPressure.Temperature, systemPressure.Temperature,
                            ControllerSettings.CommonGradient, ControllerSettings.CommonIntercept, 0.0f
                        };

                        var bufferline = new BufferLineMeasurement(now, payload);

                        speedCurrent = speed2;

                        Console.Write($"\r{fill}\r{bufferline.LogText}\r");
                        logWriter.Add(bufferline);

                        stopwatch.Stop();
                        var duration = (int)stopwatch.ElapsedMilliseconds;
                        if (duration < MeasurementSettings.RoundTripTime)
                        {
                            await Task.Delay(MeasurementSettings.RoundTripTime - duration);
                        }
                    }
                    catch (ApplicationException ex)
                    {
                        var ncdCommand = new ConverterTest2Way();
                        var result = ((SerialConverter)SerialConverter).Execute(ncdCommand);
                        
                        Console.WriteLine($"Reset serial coverter, {result}");
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
