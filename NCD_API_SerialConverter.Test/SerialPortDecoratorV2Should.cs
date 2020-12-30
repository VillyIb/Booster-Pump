using BoosterPumpConfiguration;
using Xunit;

namespace NCD_API_SerialConverter.Test
{
    public class SerialPortDecoratorV2Should
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

        [NCrunch.Framework.ExclusivelyUses("SerialPort")]
        [Fact]
        public void ScanCommand()
        {
            Init();
            Sut.Open();

            var scanCommand = new byte[] { 0xAA, 0x02, 0xC1, 0x00, 0x6D };

            Sut.Write(scanCommand);
            Sut.Write(scanCommand);
            Sut.Write(scanCommand);

            var result1 = Sut.Read();
            var result2 = Sut.Read();
            var result3 = Sut.Read();
            //var result4 = Sut.Read();
        }

        [NCrunch.Framework.ExclusivelyUses("SerialPort")]
        [Fact]
        public void Test2Way()
        {
            Init();
            Sut.Open();

            var scanCommand = new byte[] { 0xAA, 0x02, 0xFE, 0x21, 0xCB };

            Sut.Write(scanCommand);
            Sut.Write(scanCommand);
            Sut.Write(scanCommand);

            var result1 = Sut.Read();
            var result2 = Sut.Read();
            var result3 = Sut.Read();
        }
    }
}
