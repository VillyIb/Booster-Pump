using BoosterPumpLibrary.Modules;
using NCD_API_SerialConverter;
using System;
using System.Threading;

namespace BoosterPumpApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var serialPort = new SerialPortDecorator("COM4");
            var serialConverter = new SerialConverter(serialPort);

            var displayModule = new AS1115_Module(serialConverter);

            try
            {
                serialPort.Open();

                displayModule.Init();

                for (int count = 0; count < 700; count += 11)
                {
                    displayModule.SetBcdValue(count / 7f);
                    displayModule.Send();
                    Thread.Sleep(100);
                }
            }
            finally
            {
                serialPort.Dispose();
            }
        }
    }
}
