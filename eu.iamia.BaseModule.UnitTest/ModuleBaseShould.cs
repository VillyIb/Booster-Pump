using eu.iamia.BaseModule.Contract;
using NSubstitute;
using Xunit;

namespace eu.iamia.BaseModule.UnitTest
{
    public class ModuleBaseTests : ModuleBase
    {
        public ModuleBaseTests(IBridge apiToSerialBridge) : base(apiToSerialBridge)
        { }
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
    }
}
