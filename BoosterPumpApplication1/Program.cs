using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using BoosterPumpLibrary.Logger;
using Modules;
using NCD_API_SerialConverter;

namespace BoosterPumpApplication1
{
    public class Program
    {
        private static CultureInfo CultureInfo => CultureInfo.GetCultureInfo("da-DK"); //  da-DK

        private static BufferedLogWriter LogWriter { get; set; }

        private static void Log(params object[] args)
        {
            var now = DateTime.Now;
            now = new DateTime(
                now.Ticks - now.Ticks % (TimeSpan.TicksPerSecond / 10),
                now.Kind
            );

            var timestamp = now.ToString("O");
            var secondOfDay = (now.TimeOfDay.TotalMilliseconds / 100).ToString("000000", CultureInfo);

            var payload = new StringBuilder();

            foreach (var current in args)
            {
                payload.AppendFormat(CultureInfo, "{0:R}\t", current);
            }

            var row = $"{timestamp}\t{secondOfDay}\t{payload}";
            LogWriter.Add(row, now);
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
            var logfilePrefix = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),  "Dropbox\\_FlowMeasurement\\FlowController");

            var serialPort = new SerialPortDecorator("COM4");
            var serialConverter = new SerialConverter(serialPort);

            //var displayModule = new As1115Module(serialConverter);

            var manifoldPressureDifference = new AMS5812_0150_D_B_Module(serialConverter);

            var flowNorthWest = new AMS5812_0150_D_B_Module(serialConverter);

            var flowSouthEast = new AMS5812_0150_D_B_Module(serialConverter);

            var systemPressure = new AMS5812_0300_A_PressureModule(serialConverter);

            var barometerModule1 = new LPS25HB_BarometerModule(serialConverter);
            var barometerModule2 = new LPS25HB_BarometerModule(serialConverter, 1);

            var multiplexer = new TCA9546MultiplexerModule(serialConverter);

            //var speedController = new MCP4725_4_20mA_CurrentTransmitter(serialConverter);

          
            LogWriter = new BufferedLogWriter(logfilePrefix);

            try
            {
                serialPort.Open();

                Thread.Sleep(1000);

                barometerModule1.Init();
                barometerModule2.Init();

                var msMinRoundtripTime = (int)(1000 / 5);

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
                        manifoldPressureDifference.Pressure,
                        flowNorthWest.Pressure,
                        flowSouthEast.Pressure,
                        systemPressure.Pressure,
                        barometerModule1.AirPressure,
                        barometerModule2.AirPressure,
                        barometerModule1.Temperature,
                        barometerModule2.Temperature
                    );

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
