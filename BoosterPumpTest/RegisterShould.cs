using System;
using BoosterPumpLibrary.Settings;
using Xunit;

namespace BoosterPumpTest
{
    public class RegisterShould
    {
        public Register Sut;

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
            Sut = new Register(0x00, "64 bit register", 8);

            AlfaSetting = Sut.GetOrCreateSubRegister(1, 7, "Alfa"); // 0
            BravoSetting = Sut.GetOrCreateSubRegister(2, 6, "Bravo"); // 1..2
            CharlieSetting = Sut.GetOrCreateSubRegister(3, 5, "Charlie"); // 3..5
            DeltaSetting = Sut.GetOrCreateSubRegister(4, 4, "Delta"); // 4..7
            EchoSetting = Sut.GetOrCreateSubRegister(5, 3);
            FoxtrotSetting = Sut.GetOrCreateSubRegister(6, 2);
            GolfSetting = Sut.GetOrCreateSubRegister(7, 1);
            HotelSetting = Sut.GetOrCreateSubRegister(8, 0);

            IndiaSetting = Sut.GetOrCreateSubRegister(16, 4, "India");
            JulietSetting = Sut.GetOrCreateSubRegister(24, 2, "Juliet");
            KiloSetting = Sut.GetOrCreateSubRegister(32, 0, "Kilo");
            LimaSetting = Sut.GetOrCreateSubRegister(64, 0, "Lima");
        }

        [Fact]
        public void StoreMaxValuefor1BitsWithOffsett()
        {
            AlfaSetting.Value = 1;
            Assert.Equal((ulong)0b1000_0000, Sut.Value);
        }

        [Fact]
        public void StoreMaxValuefor2BitsWithOffsett()
        {
            BravoSetting.Value = 3;
            Assert.Equal((ulong)0b1100_0000, Sut.Value);
        }

        [Fact]
        public void StoreMaxValuefor3BitsWithOffsett()
        {
            CharlieSetting.Value = 7;
            Assert.Equal((ulong)0b1110_0000, Sut.Value);
        }

        [Fact]
        public void StoreMaxValuefor4BitsWithOffsett()
        {
            DeltaSetting.Value = 15;
            Assert.Equal((ulong)0b1111_0000, Sut.Value);
        }

        [Fact]
        public void StoreMaxValuefor5BitsWithOffsett()
        {
            EchoSetting.Value = 31;
            Assert.Equal((ulong)0b1111_1000, Sut.Value);
        }

        [Fact]
        public void StoreMaxValuefor6BitsWithOffsett()
        {
            FoxtrotSetting.Value = 63;
            Assert.Equal((ulong)0b1111_1100, Sut.Value);
        }

        [Fact]
        public void StoreMaxValuefor7BitsWithOffsett()
        {
            GolfSetting.Value = 127;
            Assert.Equal((ulong)0b1111_1110, Sut.Value);
        }

        [Fact]
        public void StoreMaxValuefor8Bits()
        {
            HotelSetting.Value = 255;
            Assert.Equal((ulong)0b1111_1111, Sut.Value);
        }

        [Fact]
        public void StoreMaxValuefor16BitsWithOffsett()
        {
            IndiaSetting.Value = ushort.MaxValue;
            Assert.Equal((ulong)0b1111_1111_1111_1111_0000, Sut.Value);
        }

        [Fact]
        public void StoreMaxValuefor12BitsWithOffsett()
        {
            JulietSetting.Value = (1 << 12) - 1;
            Assert.Equal((ulong)0b0111_1111_1111_100, Sut.Value);
        }

        [Fact]
        public void StoreMaxValuefor32Bits()
        {
            // ReSharper disable once InconsistentNaming
            const uint Value = uint.MaxValue;
            KiloSetting.Value = Value;
            Assert.Equal(0b1111_1111_1111_1111_1111_1111_1111_1111, Sut.Value);
        }

        [Fact]
        public void StoreMaxValuefor64Bits()
        {
            const ulong Value = ulong.MaxValue;
            LimaSetting.Value = Value;
            Assert.Equal(0b1111_1111_1111_1111_1111_1111_1111_1111_1111_1111_1111_1111_1111_1111_1111_1111, Sut.Value);
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
