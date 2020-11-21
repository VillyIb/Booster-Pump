using BoosterPumpLibrary.Contracts;
using NSubstitute;
using Xunit;
using BoosterPumpLibrary.Commands;
using Modules;

namespace ModulesTest
{
    public class As1115ModuleShould
    {
        private readonly As1115Module Sut;
        private readonly ISerialConverter FakeSerialPort;

        public As1115ModuleShould()
        {
            FakeSerialPort = Substitute.For<ISerialConverter>();
            Sut = new As1115Module(FakeSerialPort);
        }

        [Fact]
        public void SendInitSequenceWhenCallingInitAndSend()
        {
            Sut.Init();
            Sut.Send();

            FakeSerialPort.Received(3).Execute(Arg.Any<WriteCommand>());

            Received.InOrder(() =>
            {
                FakeSerialPort.Received().Execute(Arg.Is<WriteCommand>(c => c.I2CDataAsHex == "00 0C 01 "));
                FakeSerialPort.Received().Execute(Arg.Is<WriteCommand>(c => c.I2CDataAsHex == "00 0E 00 "));
                //                                                                                    98 0A 0B
                FakeSerialPort.Received().Execute(Arg.Is<WriteCommand>(c => c.I2CDataAsHex == "00 09 07 0F 02 "));
            });
        }

        [Fact]
        public void SettingIllegalValueShowEee()
        {
            Sut.SetBcdValue(-100f); // 'EEE'
            FakeSerialPort.Received(1).Execute(Arg.Any<WriteCommand>());
            FakeSerialPort.Received().Execute(Arg.Is<WriteCommand>(c => c.I2CDataAsHex == "00 01 0B 0B 0B "));
        }

        [Fact]
        public void SettingMinus98ShowSame()
        {
            Sut.SetBcdValue(-98f); // '-98'
            FakeSerialPort.Received(1).Execute(Arg.Any<WriteCommand>());
            FakeSerialPort.Received().Execute(Arg.Is<WriteCommand>(c => c.I2CDataAsHex == "00 01 0A 09 08 "));

        }

        [Fact]
        public void SettingMinus7point5ValueShows_7dot5()
        {
            Sut.SetBcdValue(-7.5f); // '-7.5'
            FakeSerialPort.Received(1).Execute(Arg.Any<WriteCommand>());
            FakeSerialPort.Received().Execute(Arg.Is<WriteCommand>(c => c.I2CDataAsHex == "00 01 0A 87 05 "));
        }

        [Fact]
        public void SettingMinus75ValueShows_75()
        {
            Sut.SetBcdValue(-75f); // '-75'
            FakeSerialPort.Received(1).Execute(Arg.Any<WriteCommand>());
            FakeSerialPort.Received().Execute(Arg.Is<WriteCommand>(c => c.I2CDataAsHex == "00 01 0A 07 05 "));
        }

        [Fact]
        public void SettingZeroValueShows000()
        {
            Sut.SetBcdValue(0f); // '0.00'
            FakeSerialPort.Received(1).Execute(Arg.Any<WriteCommand>());
            FakeSerialPort.Received().Execute(Arg.Is<WriteCommand>(c => c.I2CDataAsHex == "00 01 80 00 00 "));
        }

        [Fact]
        public void Setting1dot23ValueShowsSequence()
        {
            Sut.SetBcdValue(1.23f); // '1.23'
            FakeSerialPort.Received(1).Execute(Arg.Any<WriteCommand>());
            FakeSerialPort.Received().Execute(Arg.Is<WriteCommand>(c => c.I2CDataAsHex == "00 01 81 02 03 "));
        }

        [Fact]
        public void Setting12dot3ValueShowsSequence()
        {
            Sut.SetBcdValue(12.3f); // '12.3'
            //_FakeSerialPort.Received(1).Execute(Arg.Any<WriteCommand>());
            FakeSerialPort.Received().Execute(Arg.Is<WriteCommand>(c => c.I2CDataAsHex == "00 01 01 82 03 "));
        }

        [Fact]
        public void Setting999Shows999()
        {
            Sut.SetBcdValue(999f); // '999'
            FakeSerialPort.Received(1).Execute(Arg.Any<WriteCommand>());
            FakeSerialPort.Received().Execute(Arg.Is<WriteCommand>(c => c.I2CDataAsHex == "00 01 09 09 09 "));
        }

        [Fact]
        public void SettingHexAbc()
        {
            Sut.SetHexValue(new byte[] { 0x0A, 0x0B, 0x0C }); // 'ABC'
            Sut.Send();
            FakeSerialPort.Received(1).Execute(Arg.Any<WriteCommand>());
            FakeSerialPort.Received().Execute(Arg.Is<WriteCommand>(c => c.I2CDataAsHex == "00 01 0A 0B 0C "));
        }

        [Fact]
        public void SettingDigit0Intensity()
        {
            Sut.SetDigit0Intensity(0x0B);
            Sut.Send();
            FakeSerialPort.Received(1).Execute(Arg.Any<WriteCommand>());
            FakeSerialPort.Received().Execute(Arg.Is<WriteCommand>(c => c.I2CDataAsHex == "00 10 0B "));
        }

        [Fact]
        public void SettingDigit1Intensity()
        {
            Sut.SetDigit1Intensity(0x0C);
            Sut.Send();
            FakeSerialPort.Received(1).Execute(Arg.Any<WriteCommand>());
            FakeSerialPort.Received().Execute(Arg.Is<WriteCommand>(c => c.I2CDataAsHex == "00 10 C0 "));
        }

        [Fact]
        public void SettingDigit2Intensity()
        {
            Sut.SetDigit2Intensity(0x0D);
            Sut.Send();
            FakeSerialPort.Received(1).Execute(Arg.Any<WriteCommand>());
            FakeSerialPort.Received().Execute(Arg.Is<WriteCommand>(c => c.I2CDataAsHex == "00 11 0D "));
        }

        [Fact]
        public void SetAllDecodeOff()
        {
            Sut.SetNoDecoding();
            Sut.Send();
            FakeSerialPort.Received(1).Execute(Arg.Any<WriteCommand>());
            FakeSerialPort.Received().Execute(Arg.Is<WriteCommand>(c => c.I2CDataAsHex == "00 09 00 "));
        }

        [Fact]
        public void SetHexDecoding()
        {
            Sut.SetHexDecoding();
            Sut.Send();

            FakeSerialPort.Received(2).Execute(Arg.Any<WriteCommand>());

            Received.InOrder(() =>
            {
                FakeSerialPort.Execute(Arg.Is<WriteCommand>(c => c.I2CDataAsHex == "00 0E 04 "));
                FakeSerialPort.Execute(Arg.Is<WriteCommand>(c => c.I2CDataAsHex == "00 09 07 "));
            });
        }

        [Fact]
        public void SetBcdDecoding()
        {
            Sut.SetBcdDecoding();
            Sut.Send();

            FakeSerialPort.Received(2).Execute(Arg.Any<WriteCommand>());

            Received.InOrder(() =>
            {
                FakeSerialPort.Execute(Arg.Is<WriteCommand>(c => c.I2CDataAsHex == "00 0E 00 "));
                FakeSerialPort.Execute(Arg.Is<WriteCommand>(c => c.I2CDataAsHex == "00 09 07 "));
            });
        }

        [Fact]
        public void BlinkFast()
        {
            Sut.BlinkFast();
            Sut.Send();
            FakeSerialPort.Received(1).Execute(Arg.Any<WriteCommand>());
            FakeSerialPort.Received().Execute(Arg.Is<WriteCommand>(c => c.I2CDataAsHex == "00 0E 10 "));
        }

        [Fact]
        public void BlinkOff()
        {
            Sut.BlinkOff();
            Sut.Send();
            FakeSerialPort.Received(1).Execute(Arg.Any<WriteCommand>());
            FakeSerialPort.Received().Execute(Arg.Is<WriteCommand>(c => c.I2CDataAsHex == "00 0E 00 "));
        }

        [Fact]
        public void BlinkSlow()
        {
            Sut.BlinkSlow();
            Sut.Send();
            FakeSerialPort.Received(1).Execute(Arg.Any<WriteCommand>());
            FakeSerialPort.Received().Execute(Arg.Is<WriteCommand>(c => c.I2CDataAsHex == "00 0E 30 "));
        }

        [Fact]
        public void SetShutdownModeDown()
        {
            Sut.SetShutdownModeDown();
            Sut.Send();
            FakeSerialPort.Received(1).Execute(Arg.Any<WriteCommand>());
            FakeSerialPort.Received().Execute(Arg.Is<WriteCommand>(c => c.I2CDataAsHex == "00 0C 00 "));
        }

        [Fact]
        public void SetShutdownModeNormalResetFeature()
        {
            Sut.SetShutdownModeNormalResetFeature();
            Sut.Send();
            FakeSerialPort.Received(1).Execute(Arg.Any<WriteCommand>());
            FakeSerialPort.Received().Execute(Arg.Is<WriteCommand>(c => c.I2CDataAsHex == "00 0C 01 "));
        }
    }
}
