using BoosterPumpLibrary.Commands;
using BoosterPumpLibrary.Contracts;
using BoosterPumpLibrary.Modules;
using NSubstitute;
using Xunit;
using NCD_API_SerialConverter.NcdApiProtocol;

namespace BoosterPumpTest
{
    public class AMS5812_0150_D_B_ModuleShould
    {
        private readonly AMS5812_0150_D_B_Module _Sut;
        private readonly ISerialConverter _FakeSerialPort;

        public AMS5812_0150_D_B_ModuleShould()
        {
            _FakeSerialPort = Substitute.For<ISerialConverter>();
            _Sut = new AMS5812_0150_D_B_Module(_FakeSerialPort);
        }

        [Fact]
        public void SendReadSequenceCallingReadFromDevice()
        {
            IDataFromDevice returnValue = new DataFromDevice{Header = 0xAA, ByteCount = 0x04, Payload = new byte[] { 0x3F, 0xEB, 0x36, 0xE2 }, Checksum = 0xD8};
            _FakeSerialPort.Execute(Arg.Any<ReadCommand>()).Returns(returnValue);

            _Sut.ReadFromDevice();

            _FakeSerialPort.Received().Execute(Arg.Is<ReadCommand>(c => c.I2CDataAsHex == "78 04 "));

            Assert.Equal(-1.7f, _Sut.Pressure);
            Assert.Equal(20.2f, _Sut.Temperature);
        }
    }
}
