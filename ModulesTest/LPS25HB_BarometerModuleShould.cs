using eu.iamia.NCD.Bridge;
using eu.iamia.NCD.Serial.Contract;
using eu.iamia.NCD.Shared;
using Modules;
using NSubstitute;
using Xunit;
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedVariable

namespace ModulesTest
{
    // ReSharper disable once UnusedMember.Global
    public class LPS25HB_BarometerModuleShould
    {
        private readonly LPS25HB_BarometerModule Sut;
        private readonly IGateway _FakeGateway;

        public LPS25HB_BarometerModuleShould()
        {
            _FakeGateway = Substitute.For<IGateway>();
            Sut = new LPS25HB_BarometerModule(new ApiToSerialBridge(_FakeGateway));
            }

        [Fact]
        public void ReturnsSequenceWhenCallingInit()
        {
            Sut.Init();

            var x = _FakeGateway.ReceivedCalls();

            _FakeGateway.Received().Execute(Arg.Is<NcdApiProtocol>(c => c.PayloadAsHex == "BF 5C 05 "));
        }
        
        [Fact]
        public void ReturnsPressureAndTemperatureWhenCallingRead()
        {
            // Returns: Pressure XL, L, H, Temperature L, H
            var fakeReturnValue = new NcdApiProtocol(0xAA, 5,  new byte[] {0x66, 0xF6, 0x3F, 0xA0, 0xFD},0xE7);
           _FakeGateway.Execute(Arg.Any<INcdApiProtocol>()).Returns(fakeReturnValue);

           _FakeGateway.ClearReceivedCalls();
            Sut.ReadDevice();

            Assert.Equal(1023.4, Sut.AirPressure);
            Assert.Equal(41.2, Sut.Temperature);
        }
    }
}
