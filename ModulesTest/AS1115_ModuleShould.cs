using System.Collections.Generic;
using eu.iamia.BaseModule;
using eu.iamia.i2c.communication.contract;
using eu.iamia.NCD.API.Contract;
using eu.iamia.NCD.Bridge;
using eu.iamia.NCD.Shared;
using NSubstitute;
using Xunit;
using Modules.AS1115;

namespace ModulesTest
{
    public class As1115ModuleShould
    {
        private readonly As1115Module Sut;
        private readonly IGateway _FakeGateway;

        public As1115ModuleShould()
        {
            _FakeGateway = Substitute.For<IGateway>();
            var bridge = new ApiToSerialBridge(_FakeGateway);
            IOutputModule comModule = new OutputModule(bridge);

            Sut = new As1115Module(comModule);

            var fakeReturnValue = new NcdApiProtocol(new List<byte> { ModuleBase.ResponseWriteSuccess }); // OK successful transmission.
            _FakeGateway.Execute(Arg.Any<NcdApiProtocol>()).Returns(fakeReturnValue);
        }

        [Fact]
        public void SendInitSequenceWhenCallingInitAndSend()
        {
            Sut.Init();
            Sut.Send();

            _FakeGateway.Received(3).Execute(Arg.Any<NcdApiProtocol>());

            Received.InOrder(() =>
            {
                _FakeGateway.Received(1).Execute(Arg.Is<NcdApiProtocol>(c => c.PayloadAsHex == "BE 00 0C 01 ")); // ShutdownRegisterSettings.NormalOperationResetFeatureRegisterToDefaultSettings
                _FakeGateway.Received(1).Execute(Arg.Is<NcdApiProtocol>(c => c.PayloadAsHex == "BE 00 0E 00 ")); // Decoding.BCD
                //                                                                                    98 0A 0B
                _FakeGateway.Received(1).Execute(Arg.Is<NcdApiProtocol>(c => c.PayloadAsHex == "BE 00 09 07 0F 02 ")); // DecodeModeSettings.AllDigitsOn
            });
        }

        [Fact]
        public void SettingIllegalValueShowEee()
        {
            Sut.SetBcdValue(-100f); // 'EEE'
            _FakeGateway.Received(1).Execute(Arg.Any<NcdApiProtocol>());
            _FakeGateway.Received().Execute(Arg.Is<NcdApiProtocol>(c => c.PayloadAsHex == "BE 00 01 0B 0B 0B "));
        }

        [Fact]
        public void SettingMinus98ShowSame()
        {
            Sut.SetBcdValue(-98f); // '-98'
            _FakeGateway.Received(1).Execute(Arg.Any<NcdApiProtocol>());
            _FakeGateway.Received(1).Execute(Arg.Is<NcdApiProtocol>(c => c.PayloadAsHex == "BE 00 01 0A 09 08 "));

        }

        [Fact]
        public void SettingMinus7point5ValueShows_7dot5()
        {
            Sut.SetBcdValue(-7.5f); // '-7.5'
            _FakeGateway.Received(1).Execute(Arg.Any<NcdApiProtocol>());
            _FakeGateway.Received(1).Execute(Arg.Is<NcdApiProtocol>(c => c.PayloadAsHex == "BE 00 01 0A 87 05 "));
        }

        [Fact]
        public void SettingMinus75ValueShows_75()
        {
            Sut.SetBcdValue(-75f); // '-75'
            _FakeGateway.Received(1).Execute(Arg.Any<NcdApiProtocol>());
            _FakeGateway.Received(1).Execute(Arg.Is<NcdApiProtocol>(c => c.PayloadAsHex == "BE 00 01 0A 07 05 "));
        }

        [Fact]
        public void SettingZeroValueShows000()
        {
            Sut.SetBcdValue(0f); // '0.00'
            _FakeGateway.Received(1).Execute(Arg.Any<NcdApiProtocol>());
            _FakeGateway.Received(1).Execute(Arg.Is<NcdApiProtocol>(c => c.PayloadAsHex == "BE 00 01 80 00 00 "));
        }

        [Fact]
        public void Setting1dot23ValueShowsSequence()
        {
            Sut.SetBcdValue(1.23f); // '1.23'
            _FakeGateway.Received(1).Execute(Arg.Any<NcdApiProtocol>());
            _FakeGateway.Received(1).Execute(Arg.Is<NcdApiProtocol>(c => c.PayloadAsHex == "BE 00 01 81 02 03 "));
        }

        [Fact]
        public void Setting12dot3ValueShowsSequence()
        {
            Sut.SetBcdValue(12.3f); // '12.3'
            //_FakeSerialPort.Received(1).Execute(Arg.Any<NcdApiProtocol>());
            _FakeGateway.Received(1).Execute(Arg.Is<NcdApiProtocol>(c => c.PayloadAsHex == "BE 00 01 01 82 03 "));
        }

        [Fact]
        public void Setting999Shows999()
        {
            Sut.SetBcdValue(999f); // '999'
            _FakeGateway.Received(1).Execute(Arg.Any<NcdApiProtocol>());
            _FakeGateway.Received(1).Execute(Arg.Is<NcdApiProtocol>(c => c.PayloadAsHex == "BE 00 01 09 09 09 "));
        }

        [Fact]
        public void SettingHexAbc()
        {
            Sut.SetHexValue(new byte[] { 0x0A, 0x0B, 0x0C }); // 'ABC'
            Sut.Send();
            _FakeGateway.Received(1).Execute(Arg.Any<NcdApiProtocol>());
            _FakeGateway.Received(1).Execute(Arg.Is<NcdApiProtocol>(c => c.PayloadAsHex == "BE 00 01 0A 0B 0C "));
        }

        [Fact]
        public void SettingDigit0Intensity()
        {
            Sut.Digit0Intensity.Value = As1115Module.Level0xF.LevelB;
            Sut.Send();
            _FakeGateway.Received(1).Execute(Arg.Any<NcdApiProtocol>());
            _FakeGateway.Received(1).Execute(Arg.Is<NcdApiProtocol>(c => c.PayloadAsHex == "BE 00 10 00 0B ")); // {digit3 & digit2} {digit1 & digit0}
        }

        [Fact]
        public void SettingDigit1Intensity()
        {
            Sut.Digit1Intensity.Value = As1115Module.Level0xF.LevelC;
            Sut.Send();
            _FakeGateway.Received(1).Execute(Arg.Any<NcdApiProtocol>());
            _FakeGateway.Received(1).Execute(Arg.Is<NcdApiProtocol>(c => c.PayloadAsHex == "BE 00 10 00 C0 "));
        }

        [Fact]
        public void SettingDigit2Intensity()
        {
            Sut.Digit2Intensity.Value = As1115Module.Level0xF.LevelD;
            Sut.Send();
            _FakeGateway.Received(1).Execute(Arg.Any<NcdApiProtocol>());
            _FakeGateway.Received(1).Execute(Arg.Is<NcdApiProtocol>(c => c.PayloadAsHex == "BE 00 10 0D 00 "));
        }

        [Fact]
        public void SetAllDecodeOff()
        {
            Sut.DecodeMode.Value = As1115Module.DecodeModeSettings.AllDigitsOff;
            Sut.Send();
            _FakeGateway.Received(1).Execute(Arg.Any<NcdApiProtocol>());
            _FakeGateway.Received().Execute(Arg.Is<NcdApiProtocol>(c => c.PayloadAsHex == "BE 00 09 00 "));
        }

        [Fact]
        public void SetHexDecoding()
        {
            Sut.DecodeMode.Value = As1115Module.DecodeModeSettings.AllDigitsOn;
            Sut.DecodingSetting.Value = As1115Module.Decoding.HEX;
            Sut.Send();

            _FakeGateway.Received(2).Execute(Arg.Any<NcdApiProtocol>());

            Received.InOrder(() =>
            {
                _FakeGateway.Execute(Arg.Is<NcdApiProtocol>(c => c.PayloadAsHex == "BE 00 0E 04 "));
                _FakeGateway.Execute(Arg.Is<NcdApiProtocol>(c => c.PayloadAsHex == "BE 00 09 07 "));
            });
        }

        [Fact]
        public void SetBcdDecoding()
        {
            Sut.DecodeMode.Value = As1115Module.DecodeModeSettings.AllDigitsOn;
            Sut.DecodingSetting.Value = As1115Module.Decoding.BCD;
            Sut.Send();

            _FakeGateway.Received(2).Execute(Arg.Any<NcdApiProtocol>());

            Received.InOrder(() =>
            {
                _FakeGateway.Execute(Arg.Is<NcdApiProtocol>(c => c.PayloadAsHex == "BE 00 0E 00 "));
                _FakeGateway.Execute(Arg.Is<NcdApiProtocol>(c => c.PayloadAsHex == "BE 00 09 07 "));
            });
        }

        [Fact]
        public void BlinkFast()
        {
            Sut.Blink.Value = As1115Module.FlashMode.FlashFast;
            Sut.Send();
            _FakeGateway.Received(1).Execute(Arg.Any<NcdApiProtocol>());
            _FakeGateway.Received(1).Execute(Arg.Is<NcdApiProtocol>(c => c.PayloadAsHex == "BE 00 0E 10 "));
        }

        [Fact]
        public void BlinkOff()
        {
            Sut.Blink.Value = As1115Module.FlashMode.NoFlash;
            Sut.Send();
            _FakeGateway.Received(1).Execute(Arg.Any<NcdApiProtocol>());
            _FakeGateway.Received(1).Execute(Arg.Is<NcdApiProtocol>(c => c.PayloadAsHex == "BE 00 0E 00 "));
        }

        [Fact]
        public void BlinkSlow()
        {
            Sut.Blink.Value = As1115Module.FlashMode.FlashSlow;
            Sut.Send();
            _FakeGateway.Received(1).Execute(Arg.Any<NcdApiProtocol>());
            _FakeGateway.Received(1).Execute(Arg.Is<NcdApiProtocol>(c => c.PayloadAsHex == "BE 00 0E 30 "));
        }

        [Fact]
        public void SetShutdownModeDown()
        {
            Sut.ShutdownRegister.Value = As1115Module.ShutdownRegisterSettings.ShutdownModeResetFeatureRegisterToDefaultSettings;
            Sut.Send();
            _FakeGateway.Received(1).Execute(Arg.Any<NcdApiProtocol>());
            _FakeGateway.Received(1).Execute(Arg.Is<NcdApiProtocol>(c => c.PayloadAsHex == "BE 00 0C 00 "));
        }

        [Fact]
        public void SetShutdownModeNormalResetFeature()
        {
            Sut.ShutdownRegister.Value = As1115Module.ShutdownRegisterSettings.NormalOperationResetFeatureRegisterToDefaultSettings;
            Sut.Send();
            _FakeGateway.Received(1).Execute(Arg.Any<NcdApiProtocol>());
            _FakeGateway.Received(1).Execute(Arg.Is<NcdApiProtocol>(c => c.PayloadAsHex == "BE 00 0C 01 "));
        }
    }
}
