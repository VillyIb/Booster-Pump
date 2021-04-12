using eu.iamia.NCD.API;
using eu.iamia.NCD.DeviceCommunication.Contract;
using eu.iamia.NCD.Serial;
using Modules;
using NSubstitute;
using Xunit;

// ReSharper disable InconsistentNaming

namespace ModulesTest
{
    public class TCA9546MultiplexerModuleShould
    {
        private readonly TCA9546MultiplexerModule _Sut;
        private readonly IGateway _FakeSerialPort;

        public TCA9546MultiplexerModuleShould()
        {
            _FakeSerialPort = Substitute.For<IGateway>();
            _Sut = new TCA9546MultiplexerModule(_FakeSerialPort);
        }

        [Fact]
        public void SendSequenceWhenCallingSelectOpenChannels()
        {
            IDataFromDevice returnValue = new DataFromDevice ( 0xAA,  0x01,  new byte[] { 0x55 },  0x00 );
            _FakeSerialPort.Execute(Arg.Any<CommandWrite>()).Returns(returnValue);

            _Sut.SelectOpenChannels(MultiplexerChannels.Channel1 | MultiplexerChannels.Channel3);
            _FakeSerialPort.Received().Execute(Arg.Is<CommandWrite>(cmd => cmd.I2CDataAsHex == "70 00 0A "));
        }
    }
}
