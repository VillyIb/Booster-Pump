using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using BoosterPumpLibrary.Logger;
using eu.iamia.Configuration;
using Microsoft.Extensions.Configuration;
using Modules;
using NCD_API_SerialConverter;

namespace BoosterPumpApplication1
{
    [ExcludeFromCodeCoverage]

    public class Program
    {
        private static CultureInfo CultureInfo => CultureInfo.GetCultureInfo("da-DK"); //  da-DK

        private static BufferedLogWriter LogWriter { get; set; }

        private static IConfiguration Configuration;

        private static void Log(params object[] args)
        {
            var now = DateTime.UtcNow;
            now = new DateTime(
                now.Ticks - now.Ticks % (TimeSpan.TicksPerSecond / 10),
                now.Kind
            );

            var timestamp = now.ToLocalTime().ToString("O");
            var secondOfDay = (now.ToLocalTime().TimeOfDay.TotalMilliseconds / 100).ToString("000000", CultureInfo);

            var payload = new StringBuilder();

            foreach (var current in args)
            {
                payload.AppendFormat(CultureInfo, "{0:R}\t", current);
            }

            var row = $"{timestamp}\t{secondOfDay}\t{payload}";
            LogWriter.Add(row, now);

            Console.Write($"\r{row}");
        }

        //private  DirectoryInfo LocateDirectory(DirectoryInfo current, string target)
        //{
        //    var subdirectories = current.GetDirectories(target, SearchOption.AllDirectories);
        //    return subdirectories.FirstOrDefault();
        //}

        //private DirectoryInfo LocateDirectory(string target)
        //{

        //}
               
        // ReSharper disable once UnusedParameter.Local
        public static void Main(string[] args)
        {
            Configuration = ConfigurationSetup.Init();

            var subdir = Configuration["Database:SubDirectory"];
            var filePrefix = Configuration["Database:FilePrefix"];
            var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var logfilePrefix = Path.Combine(userProfile, subdir, filePrefix);

            var portName = Configuration["SerialPort:Name"];
            var serialPort = new SerialPortDecorator(null); // todo creae from service.
            var serialConverter = new SerialConverter(serialPort);

            bool.TryParse(Configuration["Controller:Enabled"], out var controllerEnabled);

            var displayModule = new As1115Module(serialConverter);

            var manifoldPressureDifference = new AMS5812_0150_D_B_Module(serialConverter);
            var manifoldPressureCorrection = float.Parse(Configuration["Measurements:ManifoldPressureCorrection"], CultureInfo.InvariantCulture);

            var flowNorthWest = new AMS5812_0150_D_B_Module(serialConverter);
            var flowNorthWestCorrection = float.Parse(Configuration["Measurements:FlowNorthWestCorrection"], CultureInfo.InvariantCulture);

            var flowSouthEast = new AMS5812_0150_D_B_Module(serialConverter);
            var flowSouthEastCorrection = float.Parse(Configuration["Measurements:FlowSouthEastCorrection"], CultureInfo.InvariantCulture);

            var systemPressure = new AMS5812_0300_A_PressureModule(serialConverter);
            var systemPressureCorrection = float.Parse(Configuration["Measurements:SystemPressureCorrection"], CultureInfo.InvariantCulture);

            var barometerModule1 = new LPS25HB_BarometerModule(serialConverter);
            var barometerModule2 = new LPS25HB_BarometerModule(serialConverter, 1);

            var multiplexer = new TCA9546MultiplexerModule(serialConverter);

            var speedController = controllerEnabled ? new MCP4725_4_20mA_CurrentTransmitterV2(serialConverter) : null;


            LogWriter = new BufferedLogWriter(null);

            try
            {
                serialPort.Open();

                Thread.Sleep(100);

                barometerModule1.Init();
                barometerModule2.Init();

                var msMinRoundtripTime = int.Parse(Configuration["Measurements:RoundTripTime"]);

                Console.WriteLine($"logfilePrefix: {logfilePrefix}");
                Console.WriteLine($"portName: {portName}");
                Console.WriteLine($"manifoldPressureCorrection: {manifoldPressureCorrection}");
                Console.WriteLine($"flowNorthWestCorrection: {flowNorthWestCorrection}");
                Console.WriteLine($"flowSouthEastCorrection: {flowSouthEastCorrection}");
                Console.WriteLine($"systemPressureCorrection: {systemPressureCorrection}");
                Console.WriteLine($"controllerEnabled: {controllerEnabled}");

                var stopwatch = new Stopwatch();
                while (true)
                {
                    stopwatch.Reset();
                    stopwatch.Start();

                    barometerModule1.ReadDevice();
                    barometerModule2.ReadDevice();

                    multiplexer.SelectOpenChannels(MultiplexerChannels.Channel0);
                    manifoldPressureDifference.ReadFromDevice();

                    multiplexer.SelectOpenChannels(MultiplexerChannels.Channel1);
                    flowNorthWest.ReadFromDevice();

                    multiplexer.SelectOpenChannels(MultiplexerChannels.Channel2);
                    flowSouthEast.ReadFromDevice();

                    multiplexer.SelectOpenChannels(MultiplexerChannels.Channel3);
                    systemPressure.ReadFromDevice();

                    Log(
                        Math.Round(manifoldPressureDifference.Pressure + manifoldPressureCorrection, 1),
                        Math.Round(flowNorthWest.Pressure + flowNorthWestCorrection, 1),
                        Math.Round(flowSouthEast.Pressure + flowSouthEastCorrection, 1),
                        Math.Round(systemPressure.Pressure - barometerModule1.AirPressure + systemPressureCorrection, 1),
                        Math.Round(barometerModule1.AirPressure, 1),
                        Math.Round(barometerModule2.AirPressure, 1),
                        Math.Round(barometerModule1.Temperature, 1),
                        Math.Round(barometerModule2.Temperature, 1)
                    );

                    if (controllerEnabled)
                    {
                        displayModule.SetBcdValue(50.0f);
                        speedController.SetSpeed(50.0f);
                    }

                    stopwatch.Stop();

                    var duration = (int)stopwatch.ElapsedMilliseconds;

                    if (duration < msMinRoundtripTime)
                    {
                        Thread.Sleep(msMinRoundtripTime - duration);
                    }
                }
            }

            finally
            {
                serialPort.Dispose();
                LogWriter.Dispose();
            }
        }
    }
}
