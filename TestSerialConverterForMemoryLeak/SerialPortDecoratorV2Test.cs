using System;
using System.Threading;
using BoosterPumpConfiguration;
using NCD_API_SerialConverter;

namespace TestSerialConverterForMemoryLeak
{
    public class SerialPortDecoratorV2Test
    {
        private SerialPortDecoratorV2 Sut;

        private SerialPortSettings Settings;

        private void Init()
        {
            Settings = new SerialPortSettings
            {
                PortName = "COM4",
                BaudRate = 115300,
                Timeout = 20000
            };
            Sut = new SerialPortDecoratorV2(Settings);
        }

        public void ScanCommand()
        {
            Init();
            Sut.Open();

            var scanCommand = new byte[] { 0xAA, 0x02, 0xC1, 0x00, 0x6D };

            var count = 1000;

            while (count-- > 0)
            {
                Sut.Write(scanCommand);
                var result1 = Sut.Read();
            }

            Sut.Write(scanCommand);
            Sut.Write(scanCommand);

            var result2 = Sut.Read();
            var result3 = Sut.Read();
            //var result4 = Sut.Read();
        }

        public void Test2WayCommand()
        {
            Init();
            Sut.Open();

            var test2WayCommand = new byte[] { 0xAA, 0x02, 0xFE, 0x21, 0xCB };

            var count = 10000;

            while (count-- > 0)
            {
                Sut.Write(test2WayCommand);
                var result1 = Sut.Read();
            }

            Sut.Write(test2WayCommand);
            Sut.Write(test2WayCommand);

            var result2 = Sut.Read();
            var result3 = Sut.Read();
            //var result4 = Sut.Read();

            Sut.Dispose();
            Sut = null;
            //GC.Collect();
        }

    }
}
