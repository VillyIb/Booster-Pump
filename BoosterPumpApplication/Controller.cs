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

namespace BoosterPumpApplication
{
    public class Controller : IController
    {
        private static CultureInfo CultureInfo => CultureInfo.GetCultureInfo("da-DK"); //  da-DK

        public MeasurementSettings MeasurementSettings { get; }

        public IServiceProvider ServiceProvider { get; }
        public ControllerSettings ControllerSettings { get; }

        public Controller(
            IServiceProvider serviceProvider,
            IOptionsSnapshot<MeasurementSettings> measurementSettings,
            IOptionsSnapshot<ControllerSettings> controllerSettings
        )
        {
            MeasurementSettings = measurementSettings.Value;
            ServiceProvider = serviceProvider;
            ControllerSettings = controllerSettings.Value;
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken, IBufferedLogWriter logger)
        {
            var stopwatch = new Stopwatch();

            var displayModule = ServiceProvider.GetRequiredService<As1115Module>();

            var manifoldPressureDifference = ServiceProvider.GetRequiredService<AMS5812_0150_D_B_Module>();

            var flowNorthWest = ServiceProvider.GetRequiredService<AMS5812_0150_D_B_Module>();

            var flowSouthEast = ServiceProvider.GetRequiredService<AMS5812_0150_D_B_Module>();

            var systemPressure = ServiceProvider.GetRequiredService<AMS5812_0300_A_PressureModule>();

            var barometerModule1 = ServiceProvider.GetRequiredService<LPS25HB_BarometerModule>();
            var barometerModule2 = ServiceProvider.GetRequiredService<LPS25HB_BarometerModule>();
            barometerModule2.SetAddressIncrement(1);

            var multiplexer = ServiceProvider.GetRequiredService<TCA9546MultiplexerModule>();

            var speedController = ServiceProvider.GetRequiredService<MCP4725_4_20mA_CurrentTransmitterV2>();

            var flowNorthWestStack = new System.Collections.Generic.Stack<double>();
            var flowSouthEastStack = new System.Collections.Generic.Stack<double>();

            var logWriter = ServiceProvider.GetRequiredService<BufferedLogWriterAsync>();

            float speed2 = 50;
            float speedCurrent = speed2;

            do
            {
                stopwatch.Reset();
                stopwatch.Start();

                barometerModule1.ReadDevice();
                barometerModule2.ReadDevice();

                multiplexer.SelectOpenChannels(MultiplexerChannels.Channel0);
                manifoldPressureDifference.ReadFromDevice();

                multiplexer.SelectOpenChannels(MultiplexerChannels.Channel1);
                flowNorthWest.ReadFromDevice();
                flowNorthWestStack.Push(flowNorthWest.Pressure + MeasurementSettings.FlowNorthWestCorrection);

                multiplexer.SelectOpenChannels(MultiplexerChannels.Channel2);
                flowSouthEast.ReadFromDevice();
                flowSouthEastStack.Push(flowSouthEast.Pressure + MeasurementSettings.FlowSouthEastCorrection);

                multiplexer.SelectOpenChannels(MultiplexerChannels.Channel3);
                systemPressure.ReadFromDevice();

                if (flowNorthWestStack.Count >= ControllerSettings.AverageSize)
                {
                    var flowNorthWestAverage = flowNorthWestStack.Average();
                    flowNorthWestStack.Pop();
                    var flowSouthEastAverage = flowSouthEastStack.Average();
                    flowSouthEastStack.Pop();


                    var flowNorthWestvalue = Math.Log10(Math.Max(0.1, flowNorthWestAverage) + 1) * 10;
                    var flowSouthEastValue = Math.Log10(Math.Max(0.1, flowSouthEastAverage) + 1) * 10;

                    var controllingFlow = flowNorthWestvalue * ControllerSettings.WestGreadient + flowSouthEastValue * ControllerSettings.EastGradient;
                    var speed1 = controllingFlow * ControllerSettings.CommonGradient + ControllerSettings.CommonIntercept;

                    speed2 = (float)Math.Min(99.9, Math.Max(ControllerSettings.MinSpeedPct, speed1));
                }

                if (Math.Abs(speedCurrent - speed2) > 0.05)
                {
                    displayModule.SetBcdValue(speed2);
                    speedController.SetSpeed(speed2);
                    speedCurrent = speed2;
                }
                var now = DateTime.UtcNow;
                now = new DateTime(
                    now.Ticks - now.Ticks % (TimeSpan.TicksPerSecond / 10),
                    now.Kind
                );

                var line = new StringBuilder();

                var timestamp = now.ToLocalTime().ToString("O");
                var secondOfDay = (now.ToLocalTime().TimeOfDay.TotalMilliseconds / 100).ToString("000000", CultureInfo);
                line.Append($"{timestamp}\t{secondOfDay}\t");

                line.AppendFormat(CultureInfo, "{ 0:R}\t", manifoldPressureDifference.Pressure + MeasurementSettings.ManifoldPressureCorrection);
                line.AppendFormat(CultureInfo, "{ 0:R}\t", flowNorthWest.Pressure + MeasurementSettings.FlowNorthWestCorrection);
                line.AppendFormat(CultureInfo, "{ 0:R}\t", flowSouthEast.Pressure + MeasurementSettings.FlowSouthEastCorrection);
                line.AppendFormat(CultureInfo, "{ 0:R}\t", systemPressure.Pressure + MeasurementSettings.SystemPressureCorrection);
                line.AppendFormat(CultureInfo, "{ 0:R}\t", speed2);
                line.AppendFormat(CultureInfo, "{ 0:R}\t", speedCurrent - speed2);
                line.AppendFormat(CultureInfo, "{ 0:R}\t", barometerModule1.AirPressure);
                line.AppendFormat(CultureInfo, "{ 0:R}\t", barometerModule2.AirPressure); ;
                line.AppendFormat(CultureInfo, "{ 0:R}\t", barometerModule1.Temperature);
                line.AppendFormat(CultureInfo, "{ 0:R}\t", barometerModule2.Temperature);
                line.AppendFormat(CultureInfo, "{ 0:R}\t", manifoldPressureDifference.Temperature);
                line.AppendFormat(CultureInfo, "{ 0:R}\t", flowNorthWest.Temperature);
                line.AppendFormat(CultureInfo, "{ 0:R}\t", flowSouthEast.Temperature);
                line.AppendFormat(CultureInfo, "{ 0:R}\t", systemPressure.Temperature);
                line.AppendFormat(CultureInfo, "{ 0:R}\t", ControllerSettings.CommonGradient);
                line.AppendFormat(CultureInfo, "{ 0:R}\t", ControllerSettings.CommonIntercept);
                line.AppendFormat(CultureInfo, "{ 0:R}\t", 0.0);

                logWriter.Add(line.ToString(), now);

                stopwatch.Stop();
                var duration = (int)stopwatch.ElapsedMilliseconds;
                if (duration < MeasurementSettings.RoundTripTime)
                {
                    await Task.Delay(MeasurementSettings.RoundTripTime - duration);
                }
            } while (!cancellationToken.IsCancellationRequested);
        }
    }
}
