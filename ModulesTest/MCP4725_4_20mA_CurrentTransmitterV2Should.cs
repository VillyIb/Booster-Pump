using System.Collections.Generic;
using eu.iamia.NCD.Bridge;
using eu.iamia.NCD.Serial.Contract;
using eu.iamia.NCD.Shared;
using NSubstitute;
using Xunit;
using Modules;
using Modules.MCP4725;

// ReSharper disable InconsistentNaming
// ReSharper disable CommentTypo

namespace ModulesTest
{
    public class MCP4725_4_20mA_CurrentTransmitterV2Should
    {
        private readonly MCP4725_4_20mA_CurrentTransmitterV2 Sut;
        private readonly IGateway FakeGateway;

        public MCP4725_4_20mA_CurrentTransmitterV2Should()
        {
            FakeGateway = Substitute.For<IGateway>();
            Sut = new(new ApiToSerialBridge(FakeGateway));

            var fakeReturnValue = new NcdApiProtocol(new List<byte> { 0x55 }); // OK successful transmission.
            FakeGateway.Execute(Arg.Any<NcdApiProtocol>()).Returns(fakeReturnValue);
        }

        [Fact]
        public void SendSequenceWhenCallingInit()
        {
            // Address         aa
            // speed                    ss s
            // ignored                      i
            // DacOrEEPROM           x
            // PowerDown              x
            var expected = "BE 60 00 60 80 00 ";
            Sut.Init();
            FakeGateway.Received(1).Execute(Arg.Is<NcdApiProtocol>(c => c.PayloadAsHex == expected));
        }

        [Fact]
        public void SendSequenceWhenSettingsSpeed10pct()
        {
            const int speedHex = 0b_0000_1010_1010_0101; // 0A_A5
            var speedPct = Sut.GetPctValue(speedHex);

            Sut.SetSpeed(speedPct);

            FakeGateway.Received(1).Execute(Arg.Is<NcdApiProtocol>(c => c.PayloadAsHex == "BE 60 00 40 AA 50 "));
        }

        [Fact]
        public void SendSequenceWhenPowerDown()
        {
            Sut.PowerDown.Value = MCP4725_4_20mA_CurrentTransmitterV2.PowerDownSettings.Resistor1k;
            Sut.Send();
            FakeGateway.Received(1).Execute(Arg.Any<NcdApiProtocol>());
            FakeGateway.Received(1).Execute(Arg.Is<NcdApiProtocol>(c => c.PayloadAsHex == "BE 60 00 02 00 00 "));
        }

        [Fact]
        public void SendSequenceWhenSetSpeedPersistent()
        {
            const int speedHex = 0b0000_1010_1010_0101;
            var speedPct = Sut.GetPctValue(speedHex);

            Sut.Init();
            Sut.SetSpeedPersistent(speedPct);

            // expected 011x_x00x, 1010_1010, 0101_xxxx => 60 AA 50
            FakeGateway.Received(1).Execute(Arg.Is<NcdApiProtocol>(c => c.PayloadAsHex == "BE 60 00 60 AA 50 "));
        }

        [Fact]
        public void VerifyMapToPct()
        {
            Assert.Equal(0.9998f, Sut.GetPctValue(4095), 4);
            Assert.Equal(0.5000f, Sut.GetPctValue(4096 / 2), 4);
            Assert.Equal(0.3333f, Sut.GetPctValue(4096 / 3), 4);
            Assert.Equal(0.6665f, Sut.GetPctValue(2 * 4096 / 3), 4);
            Assert.Equal(0.2000f, Sut.GetPctValue(4096 / 5), 4);
            Assert.Equal(0.0000f, Sut.GetPctValue(0), 4);
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
