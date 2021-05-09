using System.Linq;
using eu.iamia.NCD.API;
using eu.iamia.NCD.Bridge;
using eu.iamia.NCD.DeviceCommunication.Contract;
using eu.iamia.NCD.Serial;
using Modules;
using NSubstitute;
using Xunit;
// ReSharper disable InconsistentNaming

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
            Sut = new LPS25HB_BarometerModule(_FakeGateway, new ApiToSerialBridge(_FakeGateway));
            }

        [Fact]
        public void ReturnsSequenceWhenCallingInit()
        {
            Sut.Init();
            _FakeGateway.Received().Execute(Arg.Is<NcdApiProtocol>(c => c.PayloadAsHex == "5C 20 90 "));
        }
        
        [Fact]
        public void ReturnsPressureAndTemperatureWhenCallingRead()
        {
            // Returns: Pressure XL, L, H, Temperature L, H
            var fakeReturnValue = new NcdApiProtocol(0xAA, 5,  (new byte[] {0x66, 0xF6, 0x3F, 0xA0, 0xFD}),0xE7);
           _FakeGateway.Execute(Arg.Any<INcdApiProtocol>()).Returns(fakeReturnValue);
            Sut.ReadDevice();

            Assert.Equal(1023.4, Sut.AirPressure);
            Assert.Equal(41.2, Sut.Temperature);
        }
    }
}
