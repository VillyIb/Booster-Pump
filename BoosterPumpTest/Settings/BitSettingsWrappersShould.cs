using BoosterPumpLibrary.Settings;
using eu.iamia.i2c.communication.contract;
using Xunit;

namespace BoosterPumpLibrary.UnitTest.Settings
{
    public enum MyEnum
    {
        Zero,
        One,
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven
    }
    
    public class BitSettings2Should
    {
        private static readonly Register Register = new Register(0x0a, "Test", 4, Direction.InputAndOutput);

        private static readonly IBitSetting TreeBit = Register.GetOrCreateSubRegister(3, 0, "3bit");

        private static readonly EnumBitSettings<MyEnum> Sut = new EnumBitSettings<MyEnum>(TreeBit);

        [Fact]
        public void X1()
        {
            Register.Value = 7;
            Assert.Equal(MyEnum.Seven, Sut.Value);
        }

        [Fact]
        public void X2()
        {
            Sut.Value = MyEnum.Three;
            Assert.Equal(3UL, Register.Value);
        }


    }
}
