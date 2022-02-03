using System;
using eu.iamia.Util.Extensions;
using Xunit;

namespace eu.iamia.Util.UnitTest.Extensions
{
    public  class ConvertExtensionsShould
    {
        // 16 bit
        [Theory]
        [InlineData(0x0000, 0)]
        [InlineData(0x0010, 16)]
        [InlineData(0x0100, 256)]
        [InlineData(0x1000, 4096)]
        [InlineData(0x7FFF, 32767)]
        [InlineData(0x7FFE, 32766)]

        [InlineData(0xFFFF, -1)]
        [InlineData(0xFFFE, -2)]
        [InlineData(0x8002, -32766)]
        [InlineData(0x8001, -32767)]
        [InlineData(0x8000, -32768)]

        public void ReturnInt16(ushort unsigned, short signed)
        {
            var actual = unsigned.ToInt16();
            Assert.Equal(signed, actual);
        }

        // 16 bit
        [Theory]
        [InlineData(0x0000, 0)]
        [InlineData(0x0010, 16)]
        [InlineData(0x0100, 256)]
        [InlineData(0x1000, 4096)]
        [InlineData(0x7FFF, 32767)]
        [InlineData(0x7FFE, 32766)]

        [InlineData(0xFFFF, -1)]
        [InlineData(0xFFFE, -2)]
        [InlineData(0x8002, -32766)]
        [InlineData(0x8001, -32767)]
        [InlineData(0x8000, -32768)]

        public void ReturnUint16(ushort unsigned, short signed)
        {
            var actual = signed.ToUInt16();
            Assert.Equal(unsigned, actual);
        }

        // 24 bit
        [Theory]
        [InlineData(0x0000_0000, 0)]
        [InlineData(0x0000_0010, 16)]
        [InlineData(0x0000_0100, 256)]
        [InlineData(0x0000_1000, 4096)]
        [InlineData(0x0001_0000, 65536)]
        [InlineData(0x0010_0000, 1048576)]
        [InlineData(0x007F_FFFE, 8388606)]
        [InlineData(0x007F_FFFF, 8388607)]

        [InlineData(0x00FF_FFFF, -1)]
        [InlineData(0x00FF_FFFE, -2)]
        [InlineData(0x0080_0002, -8388606)]
        [InlineData(0x0080_0001, -8388607)]
        [InlineData(0x0080_0000, -8388608)]

        public void ReturnInt24(uint unsigned, int signed)
        {
            var actual = unsigned.ToInt24();
            Assert.Equal(signed, actual);
        }

        // 24 bit
        [Theory]
        [InlineData(0x0000_0000, 0)]
        [InlineData(0x0000_0010, 16)]
        [InlineData(0x0000_0100, 256)]
        [InlineData(0x0000_1000, 4096)]
        [InlineData(0x0001_0000, 65536)]
        [InlineData(0x0010_0000, 1048576)]
        [InlineData(0x007F_FFFE, 8388606)]
        [InlineData(0x007F_FFFF, 8388607)]

        [InlineData(0x00FF_FFFF, -1)]
        [InlineData(0x00FF_FFFE, -2)]
        [InlineData(0x0080_0002, -8388606)]
        [InlineData(0x0080_0001, -8388607)]
        [InlineData(0x0080_0000, -8388608)]

        public void ReturnUint24(uint unsigned, int signed)
        {
            var actual = signed.ToUint24();
            Assert.Equal(unsigned, actual);
        }

        [Fact]
        public void ReturnStringForUInt24Converter()
        {
            var actual = Convert.ToString(ConvertExtensions.UInt24Converter, 2);
            Assert.Equal("1000000000000000000000000", actual);
        }

        [Fact]
        public void ReturnStringForInt24MaxValue()
        {
            var actual = Convert.ToString(ConvertExtensions.Int24MaxValue, 2);
            Assert.Equal("11111111111111111111111", actual);
        }
    }
}
