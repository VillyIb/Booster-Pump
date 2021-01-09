using System;
using BoosterPumpConfiguration;
using NCD_API_SerialConverter;

namespace TestSerialConverterForMemoryLeak
{
    class Program
    {
       

        static void Main(string[] args)
        {
            for (var index = 0; index < 10; index++)
            {
                Console.WriteLine($"+Hello World!-{index}");

                var sut = new SerialPortDecoratorV2Test();
                sut.Test2WayCommand();
                
            }

            Console.WriteLine("+Hello World!-finished");
            Console.ReadLine();

        }
    }
}
