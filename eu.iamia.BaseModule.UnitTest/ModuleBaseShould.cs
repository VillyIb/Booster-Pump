using System;
using eu.iamia.NCD.API.Contract;
using NSubstitute;
using Xunit;

namespace eu.iamia.BaseModule.UnitTest
{
    public class ModuleBaseTests : ModuleBase
    {
        public ModuleBaseTests(IBridge apiToSerialBridge) : base(apiToSerialBridge)
        { }

        public override byte DefaultAddress => 0x00;
    }

    public  class ModuleBaseShould
    {
        private readonly IBridge Bridge = Substitute.For<IBridge>();

        public ModuleBaseTests Sut { get; }

        public ModuleBaseShould()
        {
            Sut = new ModuleBaseTests(Bridge);
        }

        [Fact]
        public void ThrowExceptionForNullIBridgeInConstructor()
        {
            //Assert.Throws<ArgumentNullException>(() => new OutputModuleTest(null));
        }

        #region SetAddressIncrement(int)

        [Theory]
        [InlineData(-1)]
        [InlineData(2)]
        public void ThrowExceptionForIllegalValueLow(int addressIncrement)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Sut.SetAddressIncrement(addressIncrement));
        }

        [Theory]
        [InlineData(0, 1, 1)]
        [InlineData(1, 0, 0)]
        public void ReturnCorrectAddressIncrementForSetAddressIncrement(int v1, int v2, byte expected)
        {
            Sut.SetAddressIncrement(v1);
            Sut.SetAddressIncrement(v2);
            byte actual = Sut.AddressIncrement;
            Assert.Equal(expected, actual);
        }

        #endregion
    }
}
