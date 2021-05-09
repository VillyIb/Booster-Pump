using System.Collections.Generic;
using eu.iamia.NCD.API;
using eu.iamia.NCD.Bridge;
using eu.iamia.NCD.DeviceCommunication.Contract;
using NSubstitute;
using Xunit;
using Modules;
using NcdApiProtocol = eu.iamia.NCD.Serial.NcdApiProtocol;

// ReSharper disable InconsistentNaming

namespace ModulesTest
{
    public class AMS5812_0150_D_B_ModuleShould
    {
        private readonly AMS5812_0150_D_B_Module _Sut;
        private readonly IGateway _FakeGateway;

        public AMS5812_0150_D_B_ModuleShould()
        {
            _FakeGateway = Substitute.For<IGateway>();
            _Sut = new AMS5812_0150_D_B_Module(_FakeGateway, new ApiToSerialBridge(_FakeGateway));
        }

        [Fact]
        public void SendReadSequenceCallingReadFromDevice()
        {
            var expectedPayloadAsHex = new NcdApiProtocol(new byte[] { DeviceRead.SerialCommandValue, _Sut.DefaultAddress, _Sut.LengthRequested }).PayloadAsHex;

            var fakeReturnValue = new NcdApiProtocol(new List<byte> { 0x3F, 0xEB, 0x36, 0xE2 }); // Defines Pressure and Temperature.
            _FakeGateway.Execute(Arg.Is<NcdApiProtocol>(cmd => cmd.PayloadAsHex == expectedPayloadAsHex)).Returns(fakeReturnValue);

            _Sut.ReadFromDevice();

            _FakeGateway.Received().Execute(Arg.Any<INcdApiProtocol>());
            _FakeGateway.Received().Execute(Arg.Is<NcdApiProtocol>(cmd => cmd.PayloadAsHex == "BF 78 04 "));

            Assert.Equal(-1.66f, _Sut.Pressure);
            Assert.Equal(20.21f, _Sut.Temperature);
        }
    }
}
