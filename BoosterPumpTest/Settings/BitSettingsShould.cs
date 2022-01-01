using System;
using BoosterPumpLibrary.Settings;
using Xunit;

namespace BoosterPumpLibrary.Unit.Test.Settings
{
    public class BitSettingsShould
    {
        private readonly RegisterBase ParentRegister;

        public BitSettingsShould()
        {
            ParentRegister = new Register(0x10, "_", 0x02);
        }

        [Fact]
        public void Ctor_WhenOk_PropertiesContainRightSettings()
        {
            var sut = new BitSetting(16, 2, ParentRegister, "MyDescription");

            Assert.Equal(16, sut.Size);
            Assert.Equal(0x02, sut.Offset);
            Assert.Equal("MyDescription", sut.Description);
            Assert.Equal("11_1111_1111_1111_1100", sut.MaskAsBinary());
            Assert.Equal((ulong)0xFFFF, sut.Mask);
            Assert.Equal("MyDescription, Size: 16, Offset: 2, Mask: 11_1111_1111_1111_1100, Value: 0 / 0x0000", sut.ToString());
        }

        [Fact]
        public void ReturnRightContentForToString()
        {
            var sut = new BitSetting(16, 2, ParentRegister, "MyDescription") { Value = 1234ul };

            Assert.Equal("MyDescription, Size: 16, Offset: 2, Mask: 11_1111_1111_1111_1100, Value: 1234 / 0x04D2", sut.ToString());

        }

        [Fact]
        public void ThrowExceptionWhenSettingIllegalValue()
        {
            var sut = new BitSetting(4, 2, ParentRegister, "MyDescription");

            const ulong illegalFiveBitValue = 0b1_0000;

            Assert.Throws<ArgumentOutOfRangeException>(() => sut.Value = illegalFiveBitValue);
        }


    }
}
