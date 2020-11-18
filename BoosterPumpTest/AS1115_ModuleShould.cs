using BoosterPumpLibrary.Contracts;
using NSubstitute;
using Xunit;
using BoosterPumpLibrary.Commands;
using Modules;

namespace BoosterPumpTest
{
    public class As1115ModuleShould
    {
        private readonly As1115Module _Sut;
        private readonly ISerialConverter _FakeSerialPort;

        public As1115ModuleShould()
        {
            _FakeSerialPort = Substitute.For<ISerialConverter>();
            _Sut = new As1115Module(_FakeSerialPort);
        }

        [Fact]
        public void SendInitSequenceWhenCallingInitAndSend()
        {
            _Sut.Init();
            _Sut.Send();

            _FakeSerialPort.Received(3).Execute(Arg.Any<WriteCommand>());

            Received.InOrder(() =>
            {
                _FakeSerialPort.Received().Execute(Arg.Is<WriteCommand>(c => c.I2CDataAsHex == "00 0C 01 "));
                _FakeSerialPort.Received().Execute(Arg.Is<WriteCommand>(c => c.I2CDataAsHex == "00 0E 00 "));
                //                                                                                    98 0A 0B
                _FakeSerialPort.Received().Execute(Arg.Is<WriteCommand>(c => c.I2CDataAsHex == "00 09 07 0F 02 "));
            });
        }

        [Fact]
        public void SettingIllegalValueShowEee()
        {
            _Sut.SetBcdValue(-100f); // 'EEE'
            _FakeSerialPort.Received(1).Execute(Arg.Any<WriteCommand>());
            _FakeSerialPort.Received().Execute(Arg.Is<WriteCommand>(c => c.I2CDataAsHex == "00 01 0B 0B 0B "));
        }

        [Fact]
        public void SettingMinus98ShowSame()
        {
            _Sut.SetBcdValue(-98f); // '-98'
            _FakeSerialPort.Received(1).Execute(Arg.Any<WriteCommand>());
            _FakeSerialPort.Received().Execute(Arg.Is<WriteCommand>(c => c.I2CDataAsHex == "00 01 0A 09 08 "));

        }

        [Fact]
        public void SettingMinus7point5ValueShows_7dot5()
        {
            _Sut.SetBcdValue(-7.5f); // '-7.5'
            _FakeSerialPort.Received(1).Execute(Arg.Any<WriteCommand>());
            _FakeSerialPort.Received().Execute(Arg.Is<WriteCommand>(c => c.I2CDataAsHex == "00 01 0A 87 05 "));
        }

        [Fact]
        public void SettingMinus75ValueShows_75()
        {
            _Sut.SetBcdValue(-75f); // '-75'
            _FakeSerialPort.Received(1).Execute(Arg.Any<WriteCommand>());
            _FakeSerialPort.Received().Execute(Arg.Is<WriteCommand>(c => c.I2CDataAsHex == "00 01 0A 07 05 "));
        }


        [Fact]
        public void SettingZeroValueShows000()
        {
            _Sut.SetBcdValue(0f); // '0.00'
            _FakeSerialPort.Received(1).Execute(Arg.Any<WriteCommand>());
            _FakeSerialPort.Received().Execute(Arg.Is<WriteCommand>(c => c.I2CDataAsHex == "00 01 80 00 00 "));
        }

        [Fact]
        public void Setting1dot23ValueShowsSequence()
        {
            _Sut.SetBcdValue(1.23f); // '1.23'
            _FakeSerialPort.Received(1).Execute(Arg.Any<WriteCommand>());
            _FakeSerialPort.Received().Execute(Arg.Is<WriteCommand>(c => c.I2CDataAsHex == "00 01 81 02 03 "));
        }

        [Fact]
        public void Setting12dot3ValueShowsSequence()
        {
            _Sut.SetBcdValue(12.3f); // '12.3'
            _FakeSerialPort.Received(1).Execute(Arg.Any<WriteCommand>());
            _FakeSerialPort.Received().Execute(Arg.Is<WriteCommand>(c => c.I2CDataAsHex == "00 01 01 82 03 "));
        }

        [Fact]
        public void Setting999Shows999()
        {
            _Sut.SetBcdValue(999f); // '999'
            _FakeSerialPort.Received(1).Execute(Arg.Any<WriteCommand>());
            _FakeSerialPort.Received().Execute(Arg.Is<WriteCommand>(c => c.I2CDataAsHex == "00 01 09 09 09 "));
        }

        [Fact]
        public void SettingHexAbc()
        {
            _Sut.SetHexValue(new byte[] { 0x0A, 0x0B, 0x0C }); // 'ABC'
            _Sut.Send();
            _FakeSerialPort.Received(1).Execute(Arg.Any<WriteCommand>());
            _FakeSerialPort.Received().Execute(Arg.Is<WriteCommand>(c => c.I2CDataAsHex == "00 01 0A 0B 0C "));
        }

        [Fact]
        public void SettingDigit0Intensity()
        {
            _Sut.Digit0Intensity(0x0B);
            _Sut.Send();
            _FakeSerialPort.Received(1).Execute(Arg.Any<WriteCommand>());
            _FakeSerialPort.Received().Execute(Arg.Is<WriteCommand>(c => c.I2CDataAsHex == "00 10 0B "));
        }

        [Fact]
        public void SettingDigit1Intensity()
        {
            _Sut.Digit1Intensity(0x0C);
            _Sut.Send();
            _FakeSerialPort.Received(1).Execute(Arg.Any<WriteCommand>());
            _FakeSerialPort.Received().Execute(Arg.Is<WriteCommand>(c => c.I2CDataAsHex == "00 10 C0 "));
        }

        [Fact]
        public void SettingDigit2Intensity()
        {
            _Sut.Digit2Intensity(0x0D);
            _Sut.Send();
            _FakeSerialPort.Received(1).Execute(Arg.Any<WriteCommand>());
            _FakeSerialPort.Received().Execute(Arg.Is<WriteCommand>(c => c.I2CDataAsHex == "00 11 0D "));
        }

        [Fact]
        public void SetAllDecodeOff()
        {
            _Sut.SetNoDecoding();
            _Sut.Send();
            _FakeSerialPort.Received(1).Execute(Arg.Any<WriteCommand>());
            _FakeSerialPort.Received().Execute(Arg.Is<WriteCommand>(c => c.I2CDataAsHex == "00 09 00 "));
        }

        [Fact]
        public void SetHexDecoding()
        {
            _Sut.SetHexDecoding();
            _Sut.Send();

            _FakeSerialPort.Received(2).Execute(Arg.Any<WriteCommand>());

            Received.InOrder(() =>
            {
                _FakeSerialPort.Execute(Arg.Is<WriteCommand>(c => c.I2CDataAsHex == "00 0E 04 "));
                _FakeSerialPort.Execute(Arg.Is<WriteCommand>(c => c.I2CDataAsHex == "00 09 07 "));
            });
        }

        [Fact]
        public void SetBcdDecoding()
        {
            _Sut.SetBcdDecoding();
            _Sut.Send();

            _FakeSerialPort.Received(2).Execute(Arg.Any<WriteCommand>());

            Received.InOrder(() =>
            {
                _FakeSerialPort.Execute(Arg.Is<WriteCommand>(c => c.I2CDataAsHex == "00 0E 00 "));
                _FakeSerialPort.Execute(Arg.Is<WriteCommand>(c => c.I2CDataAsHex == "00 09 07 "));
            });
        }

        [Fact]
        public void BlinkFast()
        {
            _Sut.BlinkFast();
            _Sut.Send();
            _FakeSerialPort.Received(1).Execute(Arg.Any<WriteCommand>());
            _FakeSerialPort.Received().Execute(Arg.Is<WriteCommand>(c => c.I2CDataAsHex == "00 0E 10 "));
        }

        [Fact]
        public void BlinkOff()
        {
            _Sut.BlinkOff();
            _Sut.Send();
            _FakeSerialPort.Received(1).Execute(Arg.Any<WriteCommand>());
            _FakeSerialPort.Received().Execute(Arg.Is<WriteCommand>(c => c.I2CDataAsHex == "00 0E 00 "));
        }

        [Fact]
        public void BlinkSlow()
        {
            _Sut.BlinkSlow();
            _Sut.Send();
            _FakeSerialPort.Received(1).Execute(Arg.Any<WriteCommand>());
            _FakeSerialPort.Received().Execute(Arg.Is<WriteCommand>(c => c.I2CDataAsHex == "00 0E 30 "));
        }

        [Fact]
        public void SetShutdownModeDown()
        {
            _Sut.SetShutdownModeDown();
            _Sut.Send();
            _FakeSerialPort.Received(1).Execute(Arg.Any<WriteCommand>());
            _FakeSerialPort.Received().Execute(Arg.Is<WriteCommand>(c => c.I2CDataAsHex == "00 0C 00 "));
        }

        [Fact]
        public void SetShutdownModeNormalResetFeature()
        {
            _Sut.SetShutdownModeNormalResetFeature();
            _Sut.Send();
            _FakeSerialPort.Received(1).Execute(Arg.Any<WriteCommand>());
            _FakeSerialPort.Received().Execute(Arg.Is<WriteCommand>(c => c.I2CDataAsHex == "00 0C 01 "));
        }


    }
}
