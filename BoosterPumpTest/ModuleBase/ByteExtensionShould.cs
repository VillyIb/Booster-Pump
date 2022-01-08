using BoosterPumpLibrary.ModuleBase;
using Xunit;
// ReSharper disable SuggestVarOrType_SimpleTypes

namespace BoosterPumpLibrary.UnitTest.ModuleBase
{
    public class ByteExtensionShould
    {
        private ByteExtension Sut { get; }

        public ByteExtensionShould()
        {
            Sut = new ByteExtension(0x33);
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
            ByteExtension actual = 0x33;
            Assert.Equal(Sut, actual);
        }

        [Fact]
        public void ReturnByteExtensionForAdditionOfTwoByteExtensions()
        {
            ByteExtension first = 0x11;
            ByteExtension second = 0x22;

            ByteExtension actual = first + second;

            Assert.Equal(Sut, actual);
        }

        [Fact]
        public void ReturnByteExtensionForAdditionOfByteExtensionAndByte()
        {
            ByteExtension first = 0x11;

            ByteExtension actual = first + (byte)0x22;

            Assert.Equal(Sut, actual);
        }
    }
}
