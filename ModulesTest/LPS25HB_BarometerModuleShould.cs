// ReSharper disable InconsistentNaming
// ReSharper disable UnusedVariable

namespace ModulesTest
{
    using eu.iamia.NCD.Bridge;
    using eu.iamia.NCD.Serial.Contract;
    using eu.iamia.NCD.Shared;
    using Modules;
    using NSubstitute;
    using Xunit;

    // ReSharper disable once UnusedMember.Global
    public class LPS25HB_BarometerModuleShould
    {
        private readonly LPS25HB_Barometer Sut;
        private readonly IGateway _FakeGateway;

        public LPS25HB_BarometerModuleShould()
        {
            _FakeGateway = Substitute.For<IGateway>();
            Sut = new(new ApiToSerialBridge(_FakeGateway));

            _FakeGateway.Execute(Arg.Any<NcdApiProtocol>()).Returns(NcdApiProtocol.WriteSuccess);
        }

        [Fact]
        public void CallGatewayExecuteWhenCallingInit()
        {
            Sut.Init();
            _FakeGateway.Received(1).Execute(Arg.Is<NcdApiProtocol>(c => c.PayloadAsHex == "BF 5C 05 "));
        }

        #region ReadFromDevice()

        [Fact]
        public void ReturnsPressureAndTemperatureWhenCallingRead()
        {
            // Returns: Pressure XL, L, H, Temperature L, H
            var fakeReturnValue = new NcdApiProtocol(new byte[] { 0x66, 0xF6, 0x3F, 0xA0, 0xFD });
            _FakeGateway.Execute(Arg.Any<INcdApiProtocol>()).Returns(fakeReturnValue);

            _FakeGateway.ClearReceivedCalls();
            Sut.ReadFromDevice();

            Assert.True(Sut.IsInputValid);
            Assert.Equal(1018.1, Sut.AirPressure);
            Assert.Equal(97.4, Sut.Temperature);
        }

        [Fact]
        public void ReturnInvalidInputForTimeout()
        {
            _FakeGateway.Execute(Arg.Any<INcdApiProtocol>()).Returns(NcdApiProtocol.Timeout);

            _FakeGateway.ClearReceivedCalls();
            Sut.ReadFromDevice();

            Assert.False(Sut.IsInputValid);
        }

        [Fact]
        public void ReturnInvalidInputForNoResponse()
        {
            _FakeGateway.Execute(Arg.Any<INcdApiProtocol>()).Returns(NcdApiProtocol.NoResponse);

            _FakeGateway.ClearReceivedCalls();
            Sut.ReadFromDevice();

            Assert.False(Sut.IsInputValid);
        }

        #endregion

        [Fact]
        public void ReturnSameTemperatureAsSet()
        {
            const double temp = 100.0;
            Sut.Temperature = temp;
            Assert.Equal(temp, Sut.Temperature);
        }

        [Fact]
        public void ReturnSamePressureAsSet()
        {
            const double pressure = 1000.0;
            Sut.AirPressure = pressure;
            Assert.Equal(pressure, Sut.AirPressure);
        }

        [Fact]
        public void ReturnRegisterValueForSpecificTempAndPressure()
        {
            Sut.Temperature = 100.0;
            Sut.AirPressure = 1000;
            var registerValue = Sut.Reading0X28.Value;
        }
    }
}
