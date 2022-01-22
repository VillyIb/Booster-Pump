using System.Collections.Generic;
using eu.iamia.NCD.Bridge;
using eu.iamia.NCD.Serial.Contract;
using eu.iamia.NCD.Shared;
using NSubstitute;
using Xunit;
using Modules;

// ReSharper disable InconsistentNaming

namespace ModulesTest
{
    public class AMS5812_0150_D_B_ModuleShould
    {
        private readonly AMS5812_0150_D_Pressure _Sut;
        private readonly IGateway _FakeGateway;

        public AMS5812_0150_D_B_ModuleShould()
        {
            _FakeGateway = Substitute.For<IGateway>();
            _Sut = new( new ApiToSerialBridge(_FakeGateway));
        }

        [Fact]
        public void ReadFromDevice_CallExecute_WithSpecificByteSequence_And_Expect_ReadingsBack()
        {
            var expectedPayloadAsHex = new NcdApiProtocol(new[] { (byte)I2CCommandCode.DeviceRead, _Sut.DefaultAddress, _Sut.LengthRequested }).PayloadAsHex;

            var fakeReturnValue = new NcdApiProtocol(new List<byte> { 0x3F, 0xEB, 0x36, 0xE2 }); // Defines Pressure and Temperature.
            _FakeGateway.Execute(Arg.Is<NcdApiProtocol>(cmd => cmd.PayloadAsHex == expectedPayloadAsHex)).Returns(fakeReturnValue);

            _Sut.ReadFromDevice();

            _FakeGateway.Received().Execute(Arg.Is<NcdApiProtocol>(cmd => cmd.PayloadAsHex == "BF 78 04 "));

            Assert.Equal(-1.66f, _Sut.Pressure);
            Assert.Equal(20.21f, _Sut.Temperature);
        }

        [Fact]
        public void ReturnTrueForValidResult()
        {
            var fakeReturnValue = new NcdApiProtocol(new List<byte> { 0x3F, 0xEB, 0x36, 0xE2 }); // Defines Pressure and Temperature.
            _FakeGateway.Execute(Arg.Any<NcdApiProtocol>()).Returns(fakeReturnValue);
            _Sut.ReadFromDevice();
            Assert.True(_Sut.IsInputValid);
        }

        [Fact]
        public void ReturnForForNullResponse()
        {
            var fakeReturnValue = new NcdApiProtocol(new List<byte> { 0x3F, 0xEB, 0x36, 0xE2 }); // Defines Pressure and Temperature.
            _FakeGateway.Execute(Arg.Any<NcdApiProtocol>()).Returns((NcdApiProtocol)null);
            _Sut.ReadFromDevice();
            Assert.False(_Sut.IsInputValid);
        }

        [Fact]
        public void ReturnForForTooLowPressure()
        {
            var fakeReturnValue = new NcdApiProtocol(new List<byte> { 0x0C, 0xCC, 0x36, 0xE2 }); // Defines Pressure and Temperature.
            _FakeGateway.Execute(Arg.Any<NcdApiProtocol>()).Returns(fakeReturnValue);
            _Sut.ReadFromDevice();
            Assert.False(_Sut.IsInputValid);
        }

        [Fact]
        public void ReturnForForTooHighPressure()
        {
            var fakeReturnValue = new NcdApiProtocol(new List<byte> { 0x73, 0x35, 0x36, 0xE2 }); // Defines Pressure and Temperature.
            _FakeGateway.Execute(Arg.Any<NcdApiProtocol>()).Returns(fakeReturnValue);
            _Sut.ReadFromDevice();
            Assert.False(_Sut.IsInputValid);
        }

        [Fact]
        public void ReturnForForTooLowTemperature()
        {
            var fakeReturnValue = new NcdApiProtocol(new List<byte> { 0x3F, 0xEB, 0x0C, 0xCC }); // Defines Pressure and Temperature.
            _FakeGateway.Execute(Arg.Any<NcdApiProtocol>()).Returns(fakeReturnValue);
            _Sut.ReadFromDevice();
            Assert.False(_Sut.IsInputValid); 
        }

        [Fact]
        public void ReturnForForTooHighTemperature()
        {
            var fakeReturnValue = new NcdApiProtocol(new List<byte> { 0x3F, 0xEB, 0x73, 0x35 }); // Defines Pressure and Temperature.
            _FakeGateway.Execute(Arg.Any<NcdApiProtocol>()).Returns(fakeReturnValue);
            _Sut.ReadFromDevice();
            Assert.False(_Sut.IsInputValid);
        }

    }
}
