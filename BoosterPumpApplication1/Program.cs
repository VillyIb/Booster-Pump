using System;
using System.Globalization;
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
                now.Ticks - now.Ticks % (TimeSpan.TicksPerSecond/10),
                now.Kind
            );

            var timestamp = now.ToString("O");
            var secondOfDay = now.TimeOfDay.TotalSeconds.ToString("00000", CultureInfo);

            var payload = new StringBuilder();

            foreach(var current in args)
            {
                payload.AppendFormat("{0:R}\t", current);
            }

            var row = $"{timestamp}\t{secondOfDay}\t{payload}";
            LogWriter.Add(row, now);
        }

        // ReSharper disable once UnusedParameter.Local
        public static void Main(string[] args)
        {
            var serialPort = new SerialPortDecorator("COM4");
            var serialConverter = new SerialConverter(serialPort);

            var displayModule = new As1115Module(serialConverter);

            var pressureModule1 = new AMS5812_0150_D_B_Module(serialConverter);
            var pressureModule2 = new AMS5812_0150_D_B_Module(serialConverter);
            var pressureModule3 = new AMS5812_0150_D_B_Module(serialConverter);
            var pressureModule4 = new AMS5812_0300_A_PressureModule(serialConverter);

            var barometerModule1 = new LPS25HB_BarometerModule(serialConverter);
            var baromet
                erModule2 = new LPS25HB_BarometerModule(serialConverter, 1);

            var multiplexer = new TCA9546MultiplexerModule(serialConverter);

            //var speedController = new MCP4725_4_20mA_CurrentTransmitter(serialConverter);

            LogWriter = new BufferedLogWriter();

            try
            {
                serialPort.Open();

                displayModule.Init();

                displayModule.SetBcdValue(1000);

                Thread.Sleep(1000);

                barometerModule1.Init();
                baromeerModule2.Init();


                if (false)
                {
                    //speedController.SetSpeed(0.50f);
                }

                while (true)
                {
                    barometerModule1.ReadDevice();
                    baromeerModule2.ReadDevice();

                    multiplexer.SelectOpenChannels( MultiplexerChannels.Channel0);
                    pressureModule1.ReadFromDevice();

                    multiplexer.SelectOpenChannels(MultiplexerChannels.Channel1);
                    pressureModule2.ReadFromDevice();

                    multiplexer.SelectOpenChannels(MultiplexerChannels.Channel2);
                    pressureModule3.ReadFromDevice();

                    multiplexer.SelectOpenChannels(MultiplexerChannels.Channel3);
                    pressureModule4.ReadFromDevice();

                    Log(pressureModule1.Pressure, pressureModule2.Pressure, pressureModule3.Pressure, pressureModule4.Pressure, barometerModule1.AirPressure, baromeerModule2.AirPressure, barometerModule1.Temperature);

                    Thread.Sleep(300); // limit to 1 each second in order for logger to work.
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
