using System.Collections.Generic;
using eu.iamia.BaseModule;
using eu.iamia.BaseModule.Contract;
using eu.iamia.i2c.communication.contract;
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
        private readonly IBridge FakeBridge;
        private readonly IOutputModule ComModule;

        public TCA9546MultiplexerModuleShould()
        {
            FakeGateway = Substitute.For<IGateway>();
            FakeBridge = Substitute.For<IBridge>();

            ComModule = new OutputModule(FakeBridge);

            _Sut = new TCA9546A_Multiplexer(ComModule);

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