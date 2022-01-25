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
            Assert.Equal(1018.06, Sut.AirPressure);
            Assert.Equal(97.41, Sut.Temperature);
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

        [Theory]
        [InlineData(0.0)]
        [InlineData(2000.0)]
        public void ReturnSamePressureAsSet(double airPressure)
        {
            Sut.AirPressure = airPressure;
            Assert.Equal(airPressure, Sut.AirPressure);
        }

        [Theory]
        [InlineData(100.0000, 1000.0, 0x00_6BD0_3E8000)] // temp_pressure
        [InlineData(110.7600, 2048.0, 0x00_7FFC_800000)] // temp_pressure
        [InlineData(42.5000, -2048.0, 0x00_0000_800000)] // temp_pressure
        [InlineData(-25.765, -2048.0, 0x00_8001_800000)] // temp_pressure
        public void ReturnRegisterValueForSpecificTempAndPressure(double temperature, double airPressure, ulong expected)
        {
            Sut.Temperature = temperature;
            Sut.AirPressure = airPressure;
            var registerValue = Sut.Reading0X28.Value;
            Assert.Equal(expected, registerValue);
        }

        [Theory]
        [InlineData(110.76,  2048.0, 0x00_7FFF_7FFFFF)] // temp_pressure
        [InlineData(42.500, -2048.0, 0x00_FFFF_800000)] // temp_pressure
        [InlineData(-25.77, -2048.0, 0x00_8000_800000)] // temp_pressure
        public void Reverse(double temperature, double airPressure, ulong hex)
        {
            Sut.Reading0X28.Value = hex;

            Assert.Equal(temperature, Sut.Temperature);
            Assert.Equal(airPressure, Sut.AirPressure);
        }

        [Theory]
        [InlineData(-25.76)]
        [InlineData(110.76)]
        public void ReturnSameTemperatureAsSet(double temperature)
        {
            var hex = Sut.Temperature =temperature;
            var temp = Sut.Temperature;

            Assert.Equal(temperature, temp);
        }
    }
}
