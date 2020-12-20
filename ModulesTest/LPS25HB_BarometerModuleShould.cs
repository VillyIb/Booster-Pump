using BoosterPumpLibrary.Commands;
using BoosterPumpLibrary.Contracts;
using Modules;
using NCD_API_SerialConverter.NcdApiProtocol;
using NSubstitute;
using Xunit;
// ReSharper disable InconsistentNaming

namespace ModulesTest
{
    // ReSharper disable once UnusedMember.Global
    public class LPS25HB_BarometerModuleShould
    {
        private readonly LPS25HB_BarometerModule Sut;
        private readonly ISerialConverter FakeSerialPort;

        public LPS25HB_BarometerModuleShould()
        {
            FakeSerialPort = Substitute.For<ISerialConverter>();
            Sut = new LPS25HB_BarometerModule(FakeSerialPort);
            }

        [Fact]
        public void ReturnsSequenceWhenCallingInit()
        {
            Sut.Init();
            FakeSerialPort.Received().Execute(Arg.Is<WriteCommand>(c => c.I2CDataAsHex == "5C 20 90 "));
        }

        [Fact]
        public void ReturnsPressureAndTemperatureWhenCallingRead()
        {
            // Returns: Pressure XL, L, H, Temperature L, H
            var fakeReturnValue = new DataFromDevice {Header = 0xAA, ByteCount = 5, Payload = new byte[] {0x66, 0xF6, 0x3F, 0xA0, 0xFD}, Checksum = 0xE7};
           FakeSerialPort.Execute(Arg.Any<ReadCommand>()).Returns(fakeReturnValue);
            Sut.ReadDevice();

            Assert.Equal(1023.4, Sut.AirPressure);
            Assert.Equal(41.2, Sut.Temperature);
        }
    }
}
