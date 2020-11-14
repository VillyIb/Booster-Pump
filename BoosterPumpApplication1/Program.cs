using System;
using System.Diagnostics;
using System.Threading;
using BoosterPumpLibrary.ModuleBase;
using BoosterPumpLibrary.Modules;
using NCD_API_SerialConverter;

namespace BoosterPumpApplication1
{
    public class Program
    {
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

            var baromeerModule = new LPS25HB_BarometerModule(serialConverter);

            var multiplexer = new TCA9546MultiplexerModule(serialConverter);

            try
            {
                serialPort.Open();

                displayModule.Init();

                displayModule.SetBcdValue(1000);

                Thread.Sleep(1000);

                baromeerModule.Init();

                double initial = 1000.0;

                for (var index = 0; index < 1000000; index++)
                {
                    baromeerModule.ReadDevice();

                    displayModule.SetBcdValue((float)(baromeerModule.AirPressure - initial));
                    Thread.Sleep(3000);

                    displayModule.SetBcdValue((float)baromeerModule.Temperature);
                    Thread.Sleep(3 * 1000);
                }

                if (true)
                {
                    var stopwatch = new Stopwatch();
                    stopwatch.Start();
                    for (int i = 0; i < 100; i++)
                    {
                        displayModule.SetBcdValue(111);
                        Thread.Sleep(1000);
                        multiplexer.SelectOpenChannels(TCA9546MultiplexerModule.Channel0);
                        pressureModule1.ReadFromDevice();
                        displayModule.SetBcdValue(pressureModule1.Pressure);
                        Thread.Sleep(2000);

                        displayModule.SetBcdValue(222);
                        Thread.Sleep(1000);
                        multiplexer.SelectOpenChannels(BitPattern.D1);
                        pressureModule2.ReadFromDevice();
                        displayModule.SetBcdValue(pressureModule2.Pressure);
                        Thread.Sleep(2000);

                        displayModule.SetBcdValue(333);
                        Thread.Sleep(1000);
                        multiplexer.SelectOpenChannels(BitPattern.D2);
                        pressureModule3.ReadFromDevice();
                        displayModule.SetBcdValue(pressureModule3.Pressure);
                        Thread.Sleep(2000);

                        displayModule.SetBcdValue(444);
                        Thread.Sleep(1000);
                        multiplexer.SelectOpenChannels(BitPattern.D3);
                        pressureModule4.ReadFromDevice();
                        displayModule.SetBcdValue(pressureModule4.Pressure - 1011.91f);
                        Thread.Sleep(2000);
                    }
                    stopwatch.Stop();
                    Console.WriteLine(String.Format($"{stopwatch.ElapsedMilliseconds} ms"));
                }
            }
            finally
            {
                serialPort.Dispose();
            }
        }
    }
}
