﻿using BoosterPumpLibrary.Commands;
using BoosterPumpLibrary.Contracts;
using Modules;
using NCD_API_SerialConverter.NcdApiProtocol;
using NSubstitute;
using Xunit;
// ReSharper disable InconsistentNaming

namespace ModulesTest
{
    public class TCA9546MultiplexerModuleShould
    {
        private readonly TCA9546MultiplexerModule _Sut;
        private readonly ISerialConverter _FakeSerialPort;

        public TCA9546MultiplexerModuleShould()
        {
            _FakeSerialPort = Substitute.For<ISerialConverter>();
            _Sut = new TCA9546MultiplexerModule(_FakeSerialPort);
        }

        [Fact]
        public void SendSequenceWhenCallingSelectOpenChannels()
        {
            IDataFromDevice returnValue = new DataFromDevice { Header = 0xAA, ByteCount = 0x01, Payload = new byte[] { 0x55 }, Checksum = 0x00 };
            _FakeSerialPort.Execute(Arg.Any<WriteCommand>()).Returns(returnValue);

            _Sut.SelectOpenChannels(MultiplexerChannels.Channel1 | MultiplexerChannels.Channel3);
            _FakeSerialPort.Received().Execute(Arg.Is<WriteCommand>(cmd => cmd.I2CDataAsHex == "70 00 0A "));
        }
    }
}
