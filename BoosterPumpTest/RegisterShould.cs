using System;
using System.Collections.Generic;
using System.Text;
using BoosterPumpLibrary.Settings;
using NSubstitute;
using Xunit;

namespace BoosterPumpTest
{
    public class RegisterShould
    {
        public Register sut;

        public BitSetting AlfaSetting;
        public BitSetting BravoSetting;
        public BitSetting CharlieSetting;
        public BitSetting DeltaSetting;
        public BitSetting EchoSetting;
        public BitSetting FoxtrotSetting;
        public BitSetting GolfSetting;
        public BitSetting HotelSetting;
        public BitSetting IndiaSetting;
        public BitSetting JulietSetting;

        public RegisterShould()
        {
            AlfaSetting = new BitSetting(1,0); // 0
            BravoSetting = new BitSetting (2,1); // 1..2
            CharlieSetting = new BitSetting (3,3); // 3..6

            DeltaSetting = new BitSetting (4,4); // 4..7
            EchoSetting = new BitSetting (5,3); // 3..7
            FoxtrotSetting = new BitSetting (6,2); // 2..7
            GolfSetting = new BitSetting (7,1); // 1..7
            HotelSetting = new BitSetting (8,0); // 0..7

            IndiaSetting = new BitSetting (24,4);
            JulietSetting = new BitSetting(12, 3);

            sut = new Register(0x00, "test register", 4, 
                new[] { 
                    AlfaSetting, 
                    BravoSetting, 
                    CharlieSetting, 
                    DeltaSetting, 
                    EchoSetting, 
                    FoxtrotSetting, 
                    GolfSetting, 
                    HotelSetting, 
                    IndiaSetting, 
                    JulietSetting 
                }
            );
        }

        [Fact]
        public void HaveSpecificValueSettingBsA()
        {
            AlfaSetting.Value = 1;
            Assert.Equal((ulong)0b0000_0001, sut.Value);
        }

        [Fact]
        public void HaveSpecificValueSettingBsB()
        {
            BravoSetting.Value = 3;
            Assert.Equal((ulong)0b000_0110, sut.Value);
        }

        [Fact]
        public void HaveSpecificValueSettingBsC()
        {
            CharlieSetting.Value = 7;
            Assert.Equal((ulong)0b011_1000, sut.Value);
        }

        [Fact]
        public void HaveSpecificValueSettingBsD()
        {
            DeltaSetting.Value = 15;
            Assert.Equal((ulong)0b1111_0000, sut.Value);
        }

        [Fact]
        public void HaveSpecificValueSettingE()
        {
            EchoSetting.Value = 31;
            Assert.Equal((ulong)0b1111_1000, sut.Value);
        }

        [Fact]
        public void HaveSpecificValueSettingF()
        {
            FoxtrotSetting.Value = 63;
            Assert.Equal((ulong)0b1111_1100, sut.Value);
        }

        [Fact]
        public void HaveSpecificValueSettingG()
        {
            GolfSetting.Value = 127;
            Assert.Equal((ulong)0b1111_1110, sut.Value);
        }

        [Fact]
        public void HaveSpecificValueSettingH()
        {
            HotelSetting.Value = 255;
            Assert.Equal((ulong)0b1111_1111, sut.Value);
        }

        [Fact]
        public void HaveSpecificValueSettingI()
        {
            IndiaSetting.Value = (1 << 24) - 1;
            Assert.Equal((ulong)0b1111_1111_1111_1111_1111_1111_0000, sut.Value);
        }

        [Fact]
        public void HaveSpecificValueSettingJ()
        {
            JulietSetting.Value = (1 << 12) - 1;
            Assert.Equal((ulong)0b0111_1111_1111_1000, sut.Value);
        }

        [Theory]
        [InlineData(2)]
        //[InlineData(-1)]
        public void ThrowsExceptionWhenOutOfRangeAlfa(ulong value)
        {
            Exception ex = Assert.Throws<ArgumentOutOfRangeException>(() => AlfaSetting.Value = value);
        }

        [Theory]
        [InlineData(4)]
        //[InlineData(-1)]
        public void ThrowsExceptionWhenOutOfRangeBravo(ulong value)
        {
            Exception ex = Assert.Throws<ArgumentOutOfRangeException>(() => BravoSetting.Value = value);
        }

        [Theory]
        [InlineData(8)]
        //[InlineData(-1)]
        public void ThrowsExceptionWhenOutOfRangeCharlie(ulong value)
        {
            Exception ex = Assert.Throws<ArgumentOutOfRangeException>(() => CharlieSetting.Value = value);
        }

        [Theory]
        [InlineData(256)]
        //[InlineData(-1)]
        public void ThrowsExceptionWhenOutOfRangeHotel(ulong value)
        {
            Exception ex = Assert.Throws<ArgumentOutOfRangeException>(() => HotelSetting.Value = value);
        }

        [Theory]
        [InlineData(16777216)]
        //[InlineData(-1)]
        public void ThrowsExceptionWhenOutOfRangeIndia(ulong value)
        {
            Exception ex = Assert.Throws<ArgumentOutOfRangeException>(() => IndiaSetting.Value = value);
        }
    }
}
