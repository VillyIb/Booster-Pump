using eu.iamia.Util.Extensions;
using Xunit;

namespace eu.iamia.Util.UnitTest.Extensions
{
    public  class ConvertExtensionsShould
    {
        // 16 bit
        [Theory]
        [InlineData(0xFFFF, -1)]
        [InlineData(0x7FFF, 32767)]
        [InlineData(0x8000, -32768)]
        public void ReturnInt16(ushort unsigned, short signed)
        {
            var actual = unsigned.ToInt16();
            Assert.Equal(signed, actual);
        }

        // 16 bit
        [Theory]
        [InlineData(0xFFFF, -1)]
        [InlineData(0x7FFF, 32767)]
        [InlineData(0x8000, -32768)]
        public void ReturnUint16(ushort unsigned, short signed)
        {
            var actual = signed.ToUInt16();
            Assert.Equal(unsigned, actual);
        }

        // 24 bit
        [Theory]
        [InlineData(0x00FF_FFFF, -1)]
        [InlineData(0x007F_FFFF, 8388607)]
        [InlineData(0x0080_0000, -8388608)]
        public void ReturnInt24(uint unsigned, int signed)
        {
            var actual = unsigned.ToInt24();
            Assert.Equal(signed, actual);
        }

        // 24 bit
        [Theory]
        [InlineData(0x00FF_FFFF, -1)]
        [InlineData(0x007F_FFFF, 8388607)]
        [InlineData(0x0080_0000, -8388608)]
        public void ReturnUint24(uint unsigned, int signed)
        {
            var actual = signed.ToUint24();
            Assert.Equal(unsigned, actual);
        }
    }
}
