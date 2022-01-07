using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using BoosterPumpLibrary.ModuleBase;
using BoosterPumpLibrary.Settings;
using eu.iamia.NCD.API.Contract;
using eu.iamia.NCD.Bridge;
using NSubstitute;
using Xunit;

namespace BoosterPumpLibrary.UnitTest.ModuleBase
{
    public class BaseModuleTest : BaseModuleV2
    {
        public BaseModuleTest(IBridge apiToSerialBridge) : base(apiToSerialBridge)
        {
            Timestamp = DateTime.Now;
        }

        public override byte DefaultAddress => 0b00;

        protected override IEnumerable<Register> Registers => new List<Register> 
        {
            new Register(0x01, "Register1", 8),
            new Register(0x02, "Register2", 8)
        };

        public DateTime Timestamp { get; set; }
    }

    public partial class BaseModuleShould
    {


        private IBridge bridge = Substitute.For<IBridge>();

        public BaseModuleV2 Sut { get; }

        public BaseModuleShould()
        {
            Sut = new BaseModuleTest(bridge);
        }

        [Fact]
        public void ThrowExceptionForNullIbrideInConstructor()
        {
            Assert.Throws<ArgumentNullException>(() => new BaseModuleTest(null));
        }
        #region SetAddressIncrement(int)

        [Fact]
        public void ThrowExceptionForIllegalValueLow()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Sut.SetAddressIncrement(-1));
        }

        [Fact]
        public void ThrowExceptionForIllegalValueHigh()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Sut.SetAddressIncrement(2));
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

        #region GetEnumerator()

        [Fact]
        public void x()
        {
            var enumerator = Sut.GetEnumerator();

            while (enumerator.MoveNext())
            {
                var t1 = enumerator.Current.Payload;
                var t2 = enumerator.Current.GetI2CCommandCode;
                foreach (var value in enumerator.Current.I2C_Data())
                {
                    
                }
            }

        }

        #endregion
    }
}
