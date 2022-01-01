// ReSharper disable UnusedVariable
// ReSharper disable IdentifierTypo

namespace BoosterPumpLibrary.UnitTest.Settings
{
    using System;
    using BoosterPumpLibrary.Settings;
    using Xunit;

    public class RegisterShould
    {
        public Register Sut;

        public BitSetting OneBitOffset7;
        public BitSetting TwoBitsOffset6;
        public BitSetting TreeBitsOffset5;
        public BitSetting FourBitsOffset4;
        public BitSetting FiveBitsOffset3;
        public BitSetting SixBitsOffset2;
        public BitSetting GolfSetting;
        public BitSetting EightBitsOffset0;
        public BitSetting SixteenBitsOffset4;
        public BitSetting TwentyFourBitsOffset2;
        public BitSetting ThirtyTwoBitsOffset0;
        public BitSetting SixtyFourBitsOffset0;

        public RegisterShould()
        {
            Sut = new(0x00, "64 bit register", 8); // 0000_0000_1111_1111_2222_2222_3333_3333_4444_4444_5555_5555_6666_6666_7777_7777

            // ReSharper disable once StringLiteralTypo
            OneBitOffset7 = Sut.GetOrCreateSubRegister(1, 7, "Alfa"); // 0

            TwoBitsOffset6 = Sut.GetOrCreateSubRegister(2, 6, "Bravo"); // 1..2
            TreeBitsOffset5 = Sut.GetOrCreateSubRegister(3, 5, "Charlie"); // 3..5
            FourBitsOffset4 = Sut.GetOrCreateSubRegister(4, 4, "Delta"); // 4..7
            FiveBitsOffset3 = Sut.GetOrCreateSubRegister(5, 3, "Echo");
            SixBitsOffset2 = Sut.GetOrCreateSubRegister(6, 2, "Foxtrot");
            GolfSetting = Sut.GetOrCreateSubRegister(7, 1, "Golf");
            EightBitsOffset0 = Sut.GetOrCreateSubRegister(8, 0, "Hotel");

            SixteenBitsOffset4 = Sut.GetOrCreateSubRegister(16, 4, "India");
            TwentyFourBitsOffset2 = Sut.GetOrCreateSubRegister(24, 2, "Juliet");
            ThirtyTwoBitsOffset0 = Sut.GetOrCreateSubRegister(32, 0, "Kilo");
            SixtyFourBitsOffset0 = Sut.GetOrCreateSubRegister(64, 0, "Lima");
        }

        #region static CheckRange
        [Fact]
        public void ThrowExceptionForCheckRangeMinValue()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Register.CheckRange(0, 1, 1, "test"));
        }

        [Fact]
        public void ThrowExceptionForCheckRangeMaxValue()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Register.CheckRange(1, 0, 0, "test"));
        }

        [Fact]
        public void DontThrowExceptionForCheckRangeLegalValue()
        {
            Register.CheckRange(1, 1, 1, "test");
        }
        #endregion

        #region Constructor
        [Fact]
        public void ThrowExceptionForTooLargeRegister()
        {
            Exception ex = Assert.Throws<ArgumentOutOfRangeException>(() => new Register(0x00, "Illegal", 9));
        }

        [Fact]
        public void ThrowExceptionForTooSmallRegister()
        {
            Exception ex = Assert.Throws<ArgumentOutOfRangeException>(() => new Register(0x00, "Illegal", 0));
        }

        [Fact]
        public void ReturnRegisterWithRightVauesForConstructor()
        {
            Assert.Equal(0, Sut.RegisterAddress);
            Assert.Equal(8, Sut.Size);
            Assert.Equal(0ul, Sut.Value);
            Assert.Equal("64 bit register", Sut.Description);
            Assert.False(Sut.IsOutputDirty);
            Assert.False(Sut.IsInputDirty);
        }
        #endregion

        #region GetOrCreateSubRegister
        [Fact]
        public void ReturnBitSettingWithRightValuesForGetOrCreateSubRegister()
        {
            var subregister = Sut.GetOrCreateSubRegister(4, 4, "Test");
            Assert.Equal((ushort)0b0000_1111, subregister.Mask);
            Assert.Equal(4u,subregister.Offset);
            Assert.Equal(4u,subregister.Size);
            Assert.Equal("Test", subregister.Description);
        }

        [Fact]
        public void ReturnSameInstanceForRepeatedCallsToGetOrCreateSubRegisterWithSameParameters()
        {
            var subregister1 = Sut.GetOrCreateSubRegister(4, 4, "Test");
            var subregister2 = Sut.GetOrCreateSubRegister(4, 4, "Test");

            Assert.Same(subregister1, subregister2);
        }

        [Fact]
        public void ThrowExceptionForIllegalSizeAndOffset()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Sut.GetOrCreateSubRegister(60, 5, "Test"));
        }
        #endregion

        #region Value save value
        [Theory]
        // byte:                 7         6         5         4         3         2         1         0
        [InlineData((ulong)0b0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0001, 00)]
        [InlineData((ulong)0b0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0010, 01)]
        [InlineData((ulong)0b0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0100, 02)]
        [InlineData((ulong)0b0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_1000, 03)]
        [InlineData((ulong)0b0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0001_0000, 04)]
        [InlineData((ulong)0b0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0010_0000, 05)]
        [InlineData((ulong)0b0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0100_0000, 06)]
        [InlineData((ulong)0b0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_1000_0000, 07)]
        [InlineData((ulong)0b0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0001_0000_0000, 08)]
        [InlineData((ulong)0b0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0010_0000_0000, 09)]
        [InlineData((ulong)0b0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0100_0000_0000, 10)]
        [InlineData((ulong)0b0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_1000_0000_0000, 11)]
        [InlineData((ulong)0b0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0001_0000_0000_0000, 12)]
        [InlineData((ulong)0b0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0010_0000_0000_0000, 13)]
        [InlineData((ulong)0b0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0100_0000_0000_0000, 14)]
        [InlineData((ulong)0b0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_1000_0000_0000_0000, 15)]
        [InlineData((ulong)0b0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_1000_0000_0000_0000_0000_0000, 23)]
        [InlineData((ulong)0b0000_0000_0000_0000_0000_0000_0000_0000_1000_0000_0000_0000_0000_0000_0000_0000, 31)]
        [InlineData((ulong)0b1000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000, 63)]
        public void SetRightValueForSingleBitWithOffsett(ulong expected, ushort offset)
        {
            var onebitWithOffset = Sut.GetOrCreateSubRegister(1, offset);
            onebitWithOffset.Value = 1;
            Assert.Equal(expected, Sut.Value);
        }

        [Fact]
        public void StoreMaxValuefor2BitsWithOffset()
        {
            TwoBitsOffset6.Value = 3;
            Assert.Equal((ulong)0b0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_1100_0000, Sut.Value);
        }

        [Fact]
        public void StoreMaxValuefor3BitsWithOffset()
        {
            TreeBitsOffset5.Value = 7;
            Assert.Equal((ulong)0b0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_1110_0000, Sut.Value);
        }

        [Fact]
        public void StoreMaxValuefor4BitsWithOffset()
        {
            FourBitsOffset4.Value = 15;
            Assert.Equal((ulong)0b0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_1111_0000, Sut.Value);
        }

        [Fact]
        public void StoreMaxValuefor5BitsWithOffset()
        {
            FiveBitsOffset3.Value = 31;
            Assert.Equal((ulong)0b0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_1111_1000, Sut.Value);
        }

        [Fact]
        public void StoreMaxValuefor6BitsWithOffset()
        {
            SixBitsOffset2.Value = 63;
            Assert.Equal((ulong)0b0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_1111_1100, Sut.Value);
        }

        [Fact]
        public void StoreMaxValuefor7BitsWithOffset()
        {
            GolfSetting.Value = 127;
            Assert.Equal((ulong)0b1111_1110, Sut.Value);
        }

        [Fact]
        public void StoreMaxValuefor8Bits()
        {
            EightBitsOffset0.Value = 255;
            Assert.Equal((ulong)0b1111_1111, Sut.Value);
        }

        [Fact]
        public void StoreMaxValuefor16BitsWithOffset()
        {
            SixteenBitsOffset4.Value = ushort.MaxValue;
            Assert.Equal((ulong)0b1111_1111_1111_1111_0000, Sut.Value);
        }

        [Fact]
        public void StoreMaxValuefor12BitsWithOffset()
        {
            TwentyFourBitsOffset2.Value = (1 << 12) - 1;
            Assert.Equal((ulong)0b0111_1111_1111_100, Sut.Value);
        }

        [Fact]
        public void StoreMaxValuefor32Bits()
        {
            // ReSharper disable once InconsistentNaming
            const uint Value = uint.MaxValue;
            ThirtyTwoBitsOffset0.Value = Value;
            Assert.Equal(0b1111_1111_1111_1111_1111_1111_1111_1111, Sut.Value);
        }

        [Fact]
        public void StoreMaxValuefor64Bits()
        {
            const ulong Value = ulong.MaxValue;
            SixtyFourBitsOffset0.Value = Value;
            Assert.Equal(0b1111_1111_1111_1111_1111_1111_1111_1111_1111_1111_1111_1111_1111_1111_1111_1111, Sut.Value);
        }

        [Fact]
        public void StoreMax32bitValueFor64Bits()
        {
            const uint Value = uint.MaxValue;
            SixtyFourBitsOffset0.Value = Value;
            Assert.Equal(0b1111_1111_1111_1111_1111_1111_1111_1111, Sut.Value);
        }
        #endregion

        #region Exception when setting illegal Value
        [Theory]
        [InlineData(0b10)]
        public void ThrowsExceptionWhenOutOfRangeOneBit(ulong value)
        {
            Exception ex = Assert.Throws<ArgumentOutOfRangeException>(() => OneBitOffset7.Value = value);
        }

        [Theory]
        [InlineData(0b100)]
        public void ThrowsExceptionWhenOutOfRangeTwoBits(ulong value)
        {
            Exception ex = Assert.Throws<ArgumentOutOfRangeException>(() => TwoBitsOffset6.Value = value);
        }

        [Theory]
        [InlineData(0b1000)]
        public void ThrowsExceptionWhenOutOfRangeThreeBits(ulong value)
        {
            Exception ex = Assert.Throws<ArgumentOutOfRangeException>(() => TreeBitsOffset5.Value = value);
        }

        [Theory]
        [InlineData(0b1_0000_0000)]
        public void ThrowsExceptionWhenOutOfRangeEightBits(ulong value)
        {
            Exception ex = Assert.Throws<ArgumentOutOfRangeException>(() => EightBitsOffset0.Value = value);
        }

        [Theory]
        [InlineData(0b1_0000_0000_0000_0000)]
        public void ThrowsExceptionWhenOutOfRangeSixteenBits(ulong value)
        {
            Exception ex = Assert.Throws<ArgumentOutOfRangeException>(() => SixteenBitsOffset4.Value = value);
        }
        #endregion

        #region IsOutputDirty
        [Fact]
        public void HaveOutputDirtyAfterAssigningOutputValue()
        {
            Sut.Value = 0xABCD;
            Assert.True(Sut.IsOutputDirty);
        }

        [Fact]
        public void HaveOutputNotDirtyAfterGetByteValuesToWriteToDevice()
        {
            Sut.Value = 0xABCD;
            var _ = Sut.GetByteValuesToWriteToDevice();
            Assert.False(Sut.IsOutputDirty);
        }
        #endregion

        #region GetByteValuesToWriteToDevice
        [Fact]
        public void ReturnRightSequenceFromGetByteValuesToWriteToDevice()
        {
            Sut.Value = 0xABCD;
            Assert.Equal(new byte[] { 0, 0, 0, 0, 0, 0, 0xAB, 0xCD }, Sut.GetByteValuesToWriteToDevice());
        }
        #endregion

        #region ToString()
        [Fact]
        public void ReturnRightStringFromToString()
        {
            ThirtyTwoBitsOffset0.Value = 0xFFFFFFFF;
            var stringValue = Sut.ToString();
            var expected =
                @"Alfa: 1000_0000, 1 / 0x00000001, " +
                "\r\nBravo: 1100_0000, 3 / 0x00000003, " +
                "\r\nCharlie: 1110_0000, 7 / 0x00000007, " +
                "\r\nDelta: 1111_0000, 15 / 0x0000000F, " +
                "\r\nEcho: 1111_1000, 31 / 0x0000001F, " +
                "\r\nFoxtrot: 1111_1100, 63 / 0x0000003F, " +
                "\r\nGolf: 1111_1110, 127 / 0x0000007F, " +
                "\r\nHotel: 1111_1111, 255 / 0x000000FF, " +
                "\r\nIndia: 1111_1111_1111_1111_0000, 65535 / 0x0000FFFF, " +
                "\r\nJuliet: 11_1111_1111_1111_1111_1111_1100, 16777215 / 0x00FFFFFF, " +
                "\r\nKilo: 1111_1111_1111_1111_1111_1111_1111_1111, 4294967295 / 0xFFFFFFFF, " +
                "\r\nLima: 1111_1111_1111_1111_1111_1111_1111_1111_1111_1111_1111_1111_1111_1111_1111_1111, 4294967295 / 0xFFFFFFFF, ";
            Assert.Equal(expected, stringValue);
        }
        #endregion
    }
}
