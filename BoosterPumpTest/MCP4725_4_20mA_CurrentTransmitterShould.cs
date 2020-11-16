using BoosterPumpLibrary.Contracts;
using BoosterPumpLibrary.Modules;
using NSubstitute;
using Xunit;
using BoosterPumpLibrary.Commands;

namespace BoosterPumpTest
{
    public class MCP4725_4_20mA_CurrentTransmitterShould
    {
        private readonly MCP4725_4_20mA_CurrentTransmitter Sut;
        private readonly ISerialConverter _FakeSerialPort;

        public MCP4725_4_20mA_CurrentTransmitterShould()
        {
            _FakeSerialPort = Substitute.For<ISerialConverter>();
            Sut = new MCP4725_4_20mA_CurrentTransmitter(_FakeSerialPort);
        }

        [Fact]
        public void SendSequenceWhenCallingInit()
        {
            Sut.Init();
            _FakeSerialPort.Received().Execute(Arg.Is<WriteCommand>(c => c.I2CDataAsHex == "60 00 60 80 00 "));
        }

        [Fact]
        public void SendSequenceWhenSettingsSpeed10pct()
        {
            var speedHex = 0b0000_1010_1010_0101;
            var speedPct = Sut.GetPctValute(speedHex);

            Sut.SetSpeed(speedPct);
            // expected 010x_x00x, 1010_1010, 0101_xxxx => 40 AA 50
            _FakeSerialPort.Received().Execute(Arg.Is<WriteCommand>(c => c.I2CDataAsHex == "60 00 40 AA 50 "));
        }

        [Fact]
        public void SendSequenceWhenPowerDown()
        {
            Sut.SetPowerDown();
            // expected xxxx_x01x, 
            _FakeSerialPort.Received().Execute(Arg.Is<WriteCommand>(c => c.I2CDataAsHex == "60 00 02 "));
        }

        [Fact]
        public void SendSequenceWhenSetSpeedPersistent()
        {
            var speedHex = 0b0000_1010_1010_0101;
            var speedPct = Sut.GetPctValute(speedHex);

            Sut.SetSpeedPersistent(speedPct);
            // expected 011x_x00x, 1010_1010, 0101_xxxx => 60 AA 50
            _FakeSerialPort.Received().Execute(Arg.Is<WriteCommand>(c => c.I2CDataAsHex == "60 00 60 AA 50 "));
        }

        [Fact]
        public void VefifyMaptToPct()
        {
            Assert.Equal("1.0000", Sut.GetPctValute(4096).ToString("N4"));
            Assert.Equal("0.5000", Sut.GetPctValute(4096 / 2).ToString("N4"));
            Assert.Equal("0.3333", Sut.GetPctValute(4096 / 3).ToString("N4"));
            Assert.Equal("0.6665", Sut.GetPctValute(2 * 4096 / 3).ToString("N4"));
            Assert.Equal("0.2000", Sut.GetPctValute(4096 / 5).ToString("N4"));
            Assert.Equal("0.0000", Sut.GetPctValute(0).ToString("N4"));
        }

        [Fact]
        public void VerifyMapFromPct()
        {
            Assert.Equal(4096, Sut.GetIntValue(1.0000f));
            Assert.Equal(4096 / 2, Sut.GetIntValue(0.5000f));
            Assert.Equal(4096 / 3, Sut.GetIntValue(0.3333f));
            Assert.Equal(2 * 4096 / 3, Sut.GetIntValue(0.6665f));
            Assert.Equal(4096 / 5, Sut.GetIntValue(0.2000f));
            Assert.Equal(0, Sut.GetIntValue(0));
        }
    }
}
