using System;
using BoosterPumpLibrary.Settings;
using Xunit;

namespace BoosterPumpTest
{
    public class RegisterShould
    {
        public RegisterHolding8Bytes sut;

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
        public BitSetting KiloSetting;
        public BitSetting LimaSetting;

        public RegisterShould()
        {
            sut = new RegisterHolding8Bytes(0x00, "64 bit register", 8);

            AlfaSetting = sut.CreateSubRegister(1, 7); // 0
            BravoSetting = sut.CreateSubRegister(2, 6); // 1..2
            CharlieSetting = sut.CreateSubRegister(3, 5); // 3..5
            DeltaSetting = sut.CreateSubRegister(4, 4); // 4..7
            EchoSetting = sut.CreateSubRegister(5, 3);
            FoxtrotSetting = sut.CreateSubRegister(6, 2);
            GolfSetting = sut.CreateSubRegister(7, 1);
            HotelSetting = sut.CreateSubRegister(8, 0);

            IndiaSetting = sut.CreateSubRegister(16, 4, "India");
            JulietSetting = sut.CreateSubRegister(24, 2, "Juliet");
            KiloSetting = sut.CreateSubRegister(32, 0, "Kilo");
            LimaSetting = sut.CreateSubRegister(64, 0, "Lima");
        }

        [Fact]
        public void StoreMaxValuefor1BitsWithOffsett()
        {
            AlfaSetting.Value = 1;
            Assert.Equal((ulong)0b1000_0000, sut.Value);
        }

        [Fact]
        public void StoreMaxValuefor2BitsWithOffsett()
        {
            BravoSetting.Value = 3;
            Assert.Equal((ulong)0b1100_0000, sut.Value);
        }

        [Fact]
        public void StoreMaxValuefor3BitsWithOffsett()
        {
            CharlieSetting.Value = 7;
            Assert.Equal((ulong)0b1110_0000, sut.Value);
        }

        [Fact]
        public void StoreMaxValuefor4BitsWithOffsett()
        {
            DeltaSetting.Value = 15;
            Assert.Equal((ulong)0b1111_0000, sut.Value);
        }

        [Fact]
        public void StoreMaxValuefor5BitsWithOffsett()
        {
            EchoSetting.Value = 31;
            Assert.Equal((ulong)0b1111_1000, sut.Value);
        }

        [Fact]
        public void StoreMaxValuefor6BitsWithOffsett()
        {
            FoxtrotSetting.Value = 63;
            Assert.Equal((ulong)0b1111_1100, sut.Value);
        }

        [Fact]
        public void StoreMaxValuefor7BitsWithOffsett()
        {
            GolfSetting.Value = 127;
            Assert.Equal((ulong)0b1111_1110, sut.Value);
        }

        [Fact]
        public void StoreMaxValuefor8Bits()
        {
            HotelSetting.Value = 255;
            Assert.Equal((ulong)0b1111_1111, sut.Value);
        }

        [Fact]
        public void StoreMaxValuefor16BitsWithOffsett()
        {
            IndiaSetting.Value = ushort.MaxValue;
            Assert.Equal((ulong)0b1111_1111_1111_1111_0000, sut.Value);
        }

        [Fact]
        public void StoreMaxValuefor12BitsWithOffsett()
        {
            JulietSetting.Value = (1 << 12) - 1;
            Assert.Equal((ulong)0b0111_1111_1111_100, sut.Value);
        }

        [Fact]
        public void StoreMaxValuefor32Bits()
        {
            var value = uint.MaxValue;
            KiloSetting.Value = value;
            Assert.Equal((ulong)0b1111_1111_1111_1111_1111_1111_1111_1111, sut.Value);
        }

        [Fact]
        public void StoreMaxValuefor64Bits()
        {
            var value = ulong.MaxValue;
            LimaSetting.Value = value;
            Assert.Equal((ulong)0b1111_1111_1111_1111_1111_1111_1111_1111_1111_1111_1111_1111_1111_1111_1111_1111, sut.Value);
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
