using System.Collections.Generic;
using eu.iamia.NCD.Bridge;
using eu.iamia.NCD.Serial.Contract;
using eu.iamia.NCD.Shared;
using Modules.TCA9546A;
using NSubstitute;
using Xunit;

// ReSharper disable InconsistentNaming

namespace ModulesTest
{
    public class TCA9546MultiplexerModuleShould
    {
        private readonly TCA9546A_Multiplexer _Sut;
        private readonly IGateway FakeGateway;

        public TCA9546MultiplexerModuleShould()
        {
            FakeGateway = Substitute.For<IGateway>();
            _Sut = new(new ApiToSerialBridge(FakeGateway));

            var fakeReturnValue = new NcdApiProtocol(new List<byte> { 0x55 }); // OK successful transmission.
            FakeGateway.Execute(Arg.Any<NcdApiProtocol>()).Returns(fakeReturnValue);
        }

        [Fact]
        public void SelectOpenChannels_OnSerialPort_CallExecute_WithByteSequence()
        {
            //INcdApiProtocol returnValue = new NcdApiProtocol(0xAA, 0x01, new byte[] {0x55}, 0x00);
            //FakeGateway.Execute(Arg.Any<NcdApiProtocol>()).Returns(returnValue);

            _Sut.SelectOpenChannels(MultiplexerChannels.Channel1 | MultiplexerChannels.Channel3);

            FakeGateway.Received(1).Execute(Arg.Is<NcdApiProtocol>(cmd => cmd.PayloadAsHex == "BE 70 00 0A "));
        }
    }
}