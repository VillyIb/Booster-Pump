using System;
using System.Collections.Generic;
using BoosterPumpLibrary.ModuleBase;
using BoosterPumpLibrary.Settings;
using eu.iamia.NCD.API.Contract;
using eu.iamia.NCD.Shared;
using NSubstitute;
using Xunit;

namespace BoosterPumpLibrary.UnitTest.ModuleBase
{
    public class OutputModuleTest : OutputModule
    {
        public OutputModuleTest(IBridge apiToSerialBridge) : base(apiToSerialBridge)
        {
            Timestamp = DateTime.Now;
        }

        public override byte DefaultAddress => 0b00;

        public Register Register1 = new Register(0x01, "Register1", 8, Direction.Output);
        public Register Register2 = new Register(0x02, "Register2", 8, Direction.Output);

        protected override IEnumerable<Register> Registers => new List<Register>
        {
            Register1,
            Register2
        };

        public DateTime Timestamp { get; set; }
    }

    public class BaseModuleShould
    {
        //private readonly byte[] ResponseWriteSuccess = { 0x55 };
        //private readonly byte[] ResponseError90 = { 0x5A };

        private readonly IBridge Bridge = Substitute.For<IBridge>();

        public OutputModuleTest Sut { get; }

        public BaseModuleShould()
        {
            Sut = new OutputModuleTest(Bridge);
            Bridge.Execute(Arg.Any<ICommand>()).Returns(NcdApiProtocol.WriteSuccess);
        }

        [Fact]
        public void ThrowExceptionForNullIBridgeInConstructor()
        {
            Assert.Throws<ArgumentNullException>(() => new OutputModuleTest(null));
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

        #region GetOutputEnumerator()

        [Fact]
        public void HaveElementsInEnumerator()
        {
            Sut.Register1.IsOutputDirty = true;
            Sut.Register2.IsOutputDirty = true;

            using var enumerator = Sut.GetOutputEnumerator();

            Assert.True(enumerator.MoveNext());
        }

        #endregion

        #region Send

        [Fact]
        public void DontCallExecuteForSend()
        {
            Assert.False(Sut.Register1.IsOutputDirty);

            Sut.Send();
            Bridge.Received(0).Execute(Arg.Any<ICommand>());

            Assert.False(Sut.Register1.IsOutputDirty);
        }

        [Fact]
        public void CallExecuteForSend()
        {
            Sut.Register1.IsOutputDirty = true;
            Sut.Register2.IsOutputDirty = true;
            
            Sut.Send();
            Bridge.Received(1).Execute(Arg.Any<ICommand>());

            Assert.False(Sut.Register1.IsOutputDirty);
            Assert.False(Sut.Register2.IsOutputDirty);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void CallExecuteMultipleTimesForSendWithTimeout(int retryCount)
        {
            Sut.Register1.IsOutputDirty = true;
            Sut.Register2.IsOutputDirty = true;

            Bridge.Execute(Arg.Any<ICommand>()).Returns(NcdApiProtocol.Timeout);
            
            Sut.RetryCount = retryCount;
            Sut.Send();

            Bridge.Received(Sut.RetryCount + 1).Execute(Arg.Any<ICommand>());
        }
        #endregion

        #region SendSpecificRegister

        [Fact]
        public void CallExecuteOnceForExplicitSendRegister()
        {
            Sut.Register1.IsOutputDirty = true;

            Bridge.Execute(Arg.Any<ICommand>()).Returns(NcdApiProtocol.WriteSuccess);

            Sut.SendSpecificRegister(Sut.Register1);

            Bridge.Received(1).Execute(Arg.Any<ICommand>());
        }

        #endregion
    }
}
