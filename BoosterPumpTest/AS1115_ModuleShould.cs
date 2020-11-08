using System.Linq;
using BoosterPumpLibrary.Contracts;
using NSubstitute;
using System.Collections.Generic;
using Xunit;
using BoosterPumpLibrary.Modules;
using System;
using BoosterPumpLibrary.Commands;

namespace BoosterPumpTest
{
    public class AS1115_ModuleShould
    {
        private readonly AS1115_Module sut;
        private readonly IModuleCommunication fakeSerialPort;

        public AS1115_ModuleShould()
        {
            fakeSerialPort = Substitute.For<IModuleCommunication>();
            sut = new AS1115_Module(fakeSerialPort);
        }

        [Fact]
        public void SendInitSequenceWhenCallingInitAndSend()
        {
            sut.Init();
            sut.Send();

            fakeSerialPort.Received(3).Execute(Arg.Any<WriteCommand>());

            Received.InOrder(() =>
            {
                fakeSerialPort.Received().Execute(Arg.Is<WriteCommand>(c => c.I2C_DataAsHex == "00 0C 01 "));
                fakeSerialPort.Received().Execute(Arg.Is<WriteCommand>(c => c.I2C_DataAsHex == "00 0E 00 "));
                //                                                                                    98 0A 0B
                fakeSerialPort.Received().Execute(Arg.Is<WriteCommand>(c => c.I2C_DataAsHex == "00 09 07 0F 02 "));
            });
        }

        [Fact]
        public void SettingIllegalValueShowEEE()
        {
            sut.SetBcdValue(-100f); // 'EEE'
            fakeSerialPort.Received(1).Execute(Arg.Any<WriteCommand>());
            fakeSerialPort.Received().Execute(Arg.Is<WriteCommand>(c => c.I2C_DataAsHex == "00 01 0B 0B 0B "));
        }

        [Fact]
        public void SettingMinus98ShowSame()
        {
            sut.SetBcdValue(-98f); // '-98'
            fakeSerialPort.Received(1).Execute(Arg.Any<WriteCommand>());
            fakeSerialPort.Received().Execute(Arg.Is<WriteCommand>(c => c.I2C_DataAsHex == "00 01 0A 09 08 "));

        }

        [Fact]
        public void SettingMinus7point5ValueShows_7dot5()
        {
            sut.SetBcdValue(-7.5f); // '-7.5'
            fakeSerialPort.Received(1).Execute(Arg.Any<WriteCommand>());
            fakeSerialPort.Received().Execute(Arg.Is<WriteCommand>(c => c.I2C_DataAsHex == "00 01 0A 87 05 "));
        }

        [Fact]
        public void SettingMinus75ValueShows_75()
        {
            sut.SetBcdValue(-75f); // '-75'
            fakeSerialPort.Received(1).Execute(Arg.Any<WriteCommand>());
            fakeSerialPort.Received().Execute(Arg.Is<WriteCommand>(c => c.I2C_DataAsHex == "00 01 0A 07 05 "));
        }


        [Fact]
        public void SettingZeroValueShows000()
        {
            sut.SetBcdValue(0f); // '0.00'
            fakeSerialPort.Received(1).Execute(Arg.Any<WriteCommand>());
            fakeSerialPort.Received().Execute(Arg.Is<WriteCommand>(c => c.I2C_DataAsHex == "00 01 80 00 00 "));
        }

        [Fact]
        public void Setting1dot23ValueShowsSequence()
        {
            sut.SetBcdValue(1.23f); // '1.23'
            fakeSerialPort.Received(1).Execute(Arg.Any<WriteCommand>());
            fakeSerialPort.Received().Execute(Arg.Is<WriteCommand>(c => c.I2C_DataAsHex == "00 01 81 02 03 "));
        }

        [Fact]
        public void Setting12dot3ValueShowsSequence()
        {
            sut.SetBcdValue(12.3f); // '12.3'
            fakeSerialPort.Received(1).Execute(Arg.Any<WriteCommand>());
            fakeSerialPort.Received().Execute(Arg.Is<WriteCommand>(c => c.I2C_DataAsHex == "00 01 01 82 03 "));
        }

        [Fact]
        public void Setting999Shows999()
        {
            sut.SetBcdValue(999f); // '999'
            fakeSerialPort.Received(1).Execute(Arg.Any<WriteCommand>());
            fakeSerialPort.Received().Execute(Arg.Is<WriteCommand>(c => c.I2C_DataAsHex == "00 01 09 09 09 "));
        }

        [Fact]
        public void SettingHexABC()
        {
            sut.SetHexVaue(new byte[] { 0x0A, 0x0B, 0x0C }); // 'ABC'
            sut.Send();
            fakeSerialPort.Received(1).Execute(Arg.Any<WriteCommand>());
            fakeSerialPort.Received().Execute(Arg.Is<WriteCommand>(c => c.I2C_DataAsHex == "00 01 0A 0B 0C "));
        }

        [Fact]
        public void SettingDigit0Intensity()
        {
            sut.Digit0Intensity(0x0B);
            sut.Send();
            fakeSerialPort.Received(1).Execute(Arg.Any<WriteCommand>());
            fakeSerialPort.Received().Execute(Arg.Is<WriteCommand>(c => c.I2C_DataAsHex == "00 10 0B "));
        }

        [Fact]
        public void SettingDigit1Intensity()
        {
            sut.Digit1Intensity(0x0C);
            sut.Send();
            fakeSerialPort.Received(1).Execute(Arg.Any<WriteCommand>());
            fakeSerialPort.Received().Execute(Arg.Is<WriteCommand>(c => c.I2C_DataAsHex == "00 10 C0 "));
        }

        [Fact]
        public void SettingDigit2Intensity()
        {
            sut.Digit2Intensity(0x0D);
            sut.Send();
            fakeSerialPort.Received(1).Execute(Arg.Any<WriteCommand>());
            fakeSerialPort.Received().Execute(Arg.Is<WriteCommand>(c => c.I2C_DataAsHex == "00 11 0D "));
        }

        [Fact]
        public void SetAllDecodeOff()
        {
            sut.SetNoDecoding();
            sut.Send();
            fakeSerialPort.Received(1).Execute(Arg.Any<WriteCommand>());
            fakeSerialPort.Received().Execute(Arg.Is<WriteCommand>(c => c.I2C_DataAsHex == "00 09 00 "));
        }

        [Fact]
        public void SetHexDecoding()
        {
            sut.SetHexDecoding();
            sut.Send();

            fakeSerialPort.Received(2).Execute(Arg.Any<WriteCommand>());

            Received.InOrder(() =>
            {
                fakeSerialPort.Execute(Arg.Is<WriteCommand>(c => c.I2C_DataAsHex == "00 0E 04 "));
                fakeSerialPort.Execute(Arg.Is<WriteCommand>(c => c.I2C_DataAsHex == "00 09 07 "));
            });
        }

        [Fact]
        public void SetBcdDecoding()
        {
            sut.SetBcdDecoding();
            sut.Send();

            fakeSerialPort.Received(2).Execute(Arg.Any<WriteCommand>());

            Received.InOrder(() =>
            {
                fakeSerialPort.Execute(Arg.Is<WriteCommand>(c => c.I2C_DataAsHex == "00 0E 00 "));
                fakeSerialPort.Execute(Arg.Is<WriteCommand>(c => c.I2C_DataAsHex == "00 09 07 "));
            });
        }

        [Fact]
        public void BlinkFast()
        {
            sut.BlinkFast();
            sut.Send();
            fakeSerialPort.Received(1).Execute(Arg.Any<WriteCommand>());
            fakeSerialPort.Received().Execute(Arg.Is<WriteCommand>(c => c.I2C_DataAsHex == "00 0E 10 "));
        }

        [Fact]
        public void BlinkOff()
        {
            sut.BlinkOff();
            sut.Send();
            fakeSerialPort.Received(1).Execute(Arg.Any<WriteCommand>());
            fakeSerialPort.Received().Execute(Arg.Is<WriteCommand>(c => c.I2C_DataAsHex == "00 0E 00 "));
        }

        [Fact]
        public void BlinkSlow()
        {
            sut.BlinkSlow();
            sut.Send();
            fakeSerialPort.Received(1).Execute(Arg.Any<WriteCommand>());
            fakeSerialPort.Received().Execute(Arg.Is<WriteCommand>(c => c.I2C_DataAsHex == "00 0E 30 "));
        }

        [Fact]
        public void SetShutdownModeDown()
        {
            sut.SetShutdownModeDown();
            sut.Send();
            fakeSerialPort.Received(1).Execute(Arg.Any<WriteCommand>());
            fakeSerialPort.Received().Execute(Arg.Is<WriteCommand>(c => c.I2C_DataAsHex == "00 0C 00 "));
        }

        [Fact]
        public void SetShutdownModeNormalResetFeature()
        {
            sut.SetShutdownModeNormalResetFeature();
            sut.Send();
            fakeSerialPort.Received(1).Execute(Arg.Any<WriteCommand>());
            fakeSerialPort.Received().Execute(Arg.Is<WriteCommand>(c => c.I2C_DataAsHex == "00 0C 01 "));
        }


    }
}
