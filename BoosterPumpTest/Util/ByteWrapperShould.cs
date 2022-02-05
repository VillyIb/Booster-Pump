using BoosterPumpLibrary.ModuleBase;
using BoosterPumpLibrary.Util;
using Xunit;
// ReSharper disable SuggestVarOrType_SimpleTypes

namespace BoosterPumpLibrary.UnitTest.ModuleBase
{
    public class ByteExtensionShould
    {
        private ByteWrapper Sut { get; }

        public ByteExtensionShould()
        {
            Sut = new ByteWrapper(0x33);
        }

        [Fact]
        public void ReturnByteForAssignToByte()
        {
            byte actual = Sut;
            Assert.Equal((byte)0x33, actual);
        }

        [Fact]
        public void ReturnByteExtensionForAssignToByteExtension()
        {
            ByteWrapper actual = 0x33;
            Assert.Equal(Sut, actual);
        }

        [Fact]
        public void ReturnByteExtensionForAdditionOfTwoByteExtensions()
        {
            ByteWrapper first = 0x11;
            ByteWrapper second = 0x22;

            ByteWrapper actual = first + second;

            Assert.Equal(Sut, actual);
        }

        [Fact]
        public void ReturnByteExtensionForAdditionOfByteExtensionAndByte()
        {
            ByteWrapper first = 0x11;

            ByteWrapper actual = first + (byte)0x22;

            Assert.Equal(Sut, actual);
        }
    }
}
