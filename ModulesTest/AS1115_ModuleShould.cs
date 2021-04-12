using BoosterPumpLibrary.Contracts;
using eu.iamia.NCD.API;
using eu.iamia.NCD.DeviceCommunication.Contract;
using NSubstitute;
using Xunit;
using Modules;

namespace ModulesTest
{
    public class As1115ModuleShould
    {
        private readonly As1115Module Sut;
        private readonly IGateway _FakeGateway;

        public As1115ModuleShould()
        {
            _FakeGateway = Substitute.For<IGateway>();
            Sut = new As1115Module(_FakeGateway);
        }

        [Fact]
        public void SendInitSequenceWhenCallingInitAndSend()
        {
            Sut.Init();
            Sut.Send();

            _FakeGateway.Received(3).Execute(Arg.Any<CommandWrite>());

            Received.InOrder(() =>
            {
                _FakeGateway.Received().Execute(Arg.Is<CommandWrite>(c => c.I2CDataAsHex == "00 0C 01 "));
                _FakeGateway.Received().Execute(Arg.Is<CommandWrite>(c => c.I2CDataAsHex == "00 0E 00 "));
                //                                                                                    98 0A 0B
                _FakeGateway.Received().Execute(Arg.Is<CommandWrite>(c => c.I2CDataAsHex == "00 09 07 0F 02 "));
            });
        }

        [Fact]
        public void SettingIllegalValueShowEee()
        {
            Sut.SetBcdValue(-100f); // 'EEE'
            _FakeGateway.Received(1).Execute(Arg.Any<CommandWrite>());
            _FakeGateway.Received().Execute(Arg.Is<CommandWrite>(c => c.I2CDataAsHex == "00 01 0B 0B 0B "));
        }

        [Fact]
        public void SettingMinus98ShowSame()
        {
            Sut.SetBcdValue(-98f); // '-98'
            _FakeGateway.Received(1).Execute(Arg.Any<CommandWrite>());
            _FakeGateway.Received().Execute(Arg.Is<CommandWrite>(c => c.I2CDataAsHex == "00 01 0A 09 08 "));

        }

        [Fact]
        public void SettingMinus7point5ValueShows_7dot5()
        {
            Sut.SetBcdValue(-7.5f); // '-7.5'
            _FakeGateway.Received(1).Execute(Arg.Any<CommandWrite>());
            _FakeGateway.Received().Execute(Arg.Is<CommandWrite>(c => c.I2CDataAsHex == "00 01 0A 87 05 "));
        }

        [Fact]
        public void SettingMinus75ValueShows_75()
        {
            Sut.SetBcdValue(-75f); // '-75'
            _FakeGateway.Received(1).Execute(Arg.Any<CommandWrite>());
            _FakeGateway.Received().Execute(Arg.Is<CommandWrite>(c => c.I2CDataAsHex == "00 01 0A 07 05 "));
        }

        [Fact]
        public void SettingZeroValueShows000()
        {
            Sut.SetBcdValue(0f); // '0.00'
            _FakeGateway.Received(1).Execute(Arg.Any<CommandWrite>());
            _FakeGateway.Received().Execute(Arg.Is<CommandWrite>(c => c.I2CDataAsHex == "00 01 80 00 00 "));
        }

        [Fact]
        public void Setting1dot23ValueShowsSequence()
        {
            Sut.SetBcdValue(1.23f); // '1.23'
            _FakeGateway.Received(1).Execute(Arg.Any<CommandWrite>());
            _FakeGateway.Received().Execute(Arg.Is<CommandWrite>(c => c.I2CDataAsHex == "00 01 81 02 03 "));
        }

        [Fact]
        public void Setting12dot3ValueShowsSequence()
        {
            Sut.SetBcdValue(12.3f); // '12.3'
            //_FakeSerialPort.Received(1).Execute(Arg.Any<CommandWrite>());
            _FakeGateway.Received().Execute(Arg.Is<CommandWrite>(c => c.I2CDataAsHex == "00 01 01 82 03 "));
        }

        [Fact]
        public void Setting999Shows999()
        {
            Sut.SetBcdValue(999f); // '999'
            _FakeGateway.Received(1).Execute(Arg.Any<CommandWrite>());
            _FakeGateway.Received().Execute(Arg.Is<CommandWrite>(c => c.I2CDataAsHex == "00 01 09 09 09 "));
        }

        [Fact]
        public void SettingHexAbc()
        {
            Sut.SetHexValue(new byte[] { 0x0A, 0x0B, 0x0C }); // 'ABC'
            Sut.Send();
            _FakeGateway.Received(1).Execute(Arg.Any<CommandWrite>());
            _FakeGateway.Received().Execute(Arg.Is<CommandWrite>(c => c.I2CDataAsHex == "00 01 0A 0B 0C "));
        }

        [Fact]
        public void SettingDigit0Intensity()
        {
            Sut.SetDigit0Intensity(0x0B);
            Sut.Send();
            _FakeGateway.Received(1).Execute(Arg.Any<CommandWrite>());
            _FakeGateway.Received().Execute(Arg.Is<CommandWrite>(c => c.I2CDataAsHex == "00 10 0B "));
        }

        [Fact]
        public void SettingDigit1Intensity()
        {
            Sut.SetDigit1Intensity(0x0C);
            Sut.Send();
            _FakeGateway.Received(1).Execute(Arg.Any<CommandWrite>());
            _FakeGateway.Received().Execute(Arg.Is<CommandWrite>(c => c.I2CDataAsHex == "00 10 C0 "));
        }

        [Fact]
        public void SettingDigit2Intensity()
        {
            Sut.SetDigit2Intensity(0x0D);
            Sut.Send();
            _FakeGateway.Received(1).Execute(Arg.Any<CommandWrite>());
            _FakeGateway.Received().Execute(Arg.Is<CommandWrite>(c => c.I2CDataAsHex == "00 11 0D "));
        }

        [Fact]
        public void SetAllDecodeOff()
        {
            Sut.SetNoDecoding();
            Sut.Send();
            _FakeGateway.Received(1).Execute(Arg.Any<CommandWrite>());
            _FakeGateway.Received().Execute(Arg.Is<CommandWrite>(c => c.I2CDataAsHex == "00 09 00 "));
        }

        [Fact]
        public void SetHexDecoding()
        {
            Sut.SetHexDecoding();
            Sut.Send();

            _FakeGateway.Received(2).Execute(Arg.Any<CommandWrite>());

            Received.InOrder(() =>
            {
                _FakeGateway.Execute(Arg.Is<CommandWrite>(c => c.I2CDataAsHex == "00 0E 04 "));
                _FakeGateway.Execute(Arg.Is<CommandWrite>(c => c.I2CDataAsHex == "00 09 07 "));
            });
        }

        [Fact]
        public void SetBcdDecoding()
        {
            Sut.SetBcdDecoding();
            Sut.Send();

            _FakeGateway.Received(2).Execute(Arg.Any<CommandWrite>());

            Received.InOrder(() =>
            {
                _FakeGateway.Execute(Arg.Is<CommandWrite>(c => c.I2CDataAsHex == "00 0E 00 "));
                _FakeGateway.Execute(Arg.Is<CommandWrite>(c => c.I2CDataAsHex == "00 09 07 "));
            });
        }

        [Fact]
        public void BlinkFast()
        {
            Sut.BlinkFast();
            Sut.Send();
            _FakeGateway.Received(1).Execute(Arg.Any<CommandWrite>());
            _FakeGateway.Received().Execute(Arg.Is<CommandWrite>(c => c.I2CDataAsHex == "00 0E 10 "));
        }

        [Fact]
        public void BlinkOff()
        {
            Sut.BlinkOff();
            Sut.Send();
            _FakeGateway.Received(1).Execute(Arg.Any<CommandWrite>());
            _FakeGateway.Received().Execute(Arg.Is<CommandWrite>(c => c.I2CDataAsHex == "00 0E 00 "));
        }

        [Fact]
        public void BlinkSlow()
        {
            Sut.BlinkSlow();
            Sut.Send();
            _FakeGateway.Received(1).Execute(Arg.Any<CommandWrite>());
            _FakeGateway.Received().Execute(Arg.Is<CommandWrite>(c => c.I2CDataAsHex == "00 0E 30 "));
        }

        [Fact]
        public void SetShutdownModeDown()
        {
            Sut.SetShutdownModeDown();
            Sut.Send();
            _FakeGateway.Received(1).Execute(Arg.Any<CommandWrite>());
            _FakeGateway.Received().Execute(Arg.Is<CommandWrite>(c => c.I2CDataAsHex == "00 0C 00 "));
        }

        [Fact]
        public void SetShutdownModeNormalResetFeature()
        {
            Sut.SetShutdownModeNormalResetFeature();
            Sut.Send();
            _FakeGateway.Received(1).Execute(Arg.Any<CommandWrite>());
            _FakeGateway.Received().Execute(Arg.Is<CommandWrite>(c => c.I2CDataAsHex == "00 0C 01 "));
        }
    }
}
