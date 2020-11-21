using BoosterPumpLibrary.Contracts;
using NSubstitute;
using Xunit;
using BoosterPumpLibrary.Commands;
using Modules;
// ReSharper disable InconsistentNaming
// ReSharper disable CommentTypo

namespace ModulesTest
{
    public class MCP4725_4_20mA_CurrentTransmitterV2Should
    {
        private readonly MCP4725_4_20mA_CurrentTransmitterV2 Sut;
        private readonly ISerialConverter FakeSerialPort;

        public MCP4725_4_20mA_CurrentTransmitterV2Should()
        {
            FakeSerialPort = Substitute.For<ISerialConverter>();
            Sut = new MCP4725_4_20mA_CurrentTransmitterV2(FakeSerialPort);
        }

        [Fact]
        public void SendSequenceWhenCallingInit()
        {
            Sut.Init();
            FakeSerialPort.Received().Execute(Arg.Is<WriteCommand>(c => c.I2CDataAsHex == "60 00 60 80 00 "));
        }

        [Fact]
        public void SendSequenceWhenSettingsSpeed10pct()
        {
            const int speedHex = 0b0000_1010_1010_0101;
            var speedPct = Sut.GetPctValue(speedHex);

            Sut.SetSpeed(speedPct);
            // expected 010x_x00x, 1010_1010, 0101_xxxx => 40 AA 50
            FakeSerialPort.Received().Execute(Arg.Is<WriteCommand>(c => c.I2CDataAsHex == "60 00 40 AA 50 "));
        }

        [Fact]
        public void SendSequenceWhenPowerDown()
        {
            Sut.SetPowerDown();
            FakeSerialPort.Received().Execute(Arg.Is<WriteCommand>(c => c.I2CDataAsHex == "60 00 02 00 00 "));
        }

        [Fact]
        public void SendSequenceWhenSetSpeedPersistent()
        {
            const int speedHex = 0b0000_1010_1010_0101;
            var speedPct = Sut.GetPctValue(speedHex);

            Sut.SetSpeedPersistent(speedPct);
            // expected 011x_x00x, 1010_1010, 0101_xxxx => 60 AA 50
            FakeSerialPort.Received().Execute(Arg.Is<WriteCommand>(c => c.I2CDataAsHex == "60 00 60 AA 50 "));
        }

        [Fact]
        public void VefifyMaptToPct()
        {
            Assert.Equal("0.9998", Sut.GetPctValue(4095).ToString("N4"));
            Assert.Equal("0.5000", Sut.GetPctValue(4096 / 2).ToString("N4"));
            Assert.Equal("0.3333", Sut.GetPctValue(4096 / 3).ToString("N4"));
            Assert.Equal("0.6665", Sut.GetPctValue(2 * 4096 / 3).ToString("N4"));
            Assert.Equal("0.2000", Sut.GetPctValue(4096 / 5).ToString("N4"));
            Assert.Equal("0.0000", Sut.GetPctValue(0).ToString("N4"));
        }

        [Fact]
        public void VerifyMapFromPct()
        {
            Assert.Equal(4095, Sut.GetIntValue(0.9998f));
            Assert.Equal(4096 / 2, Sut.GetIntValue(0.5000f));
            Assert.Equal(4096 / 3, Sut.GetIntValue(0.3333f));
            Assert.Equal(2 * 4096 / 3, Sut.GetIntValue(0.6665f));
            Assert.Equal(4096 / 5, Sut.GetIntValue(0.2000f));
            Assert.Equal(0, Sut.GetIntValue(0));
        }
    }
}
