﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using BoosterPumpLibrary.ModuleBase;
using BoosterPumpLibrary.Settings;
using EnsureThat.Enforcers;
using eu.iamia.NCD.API;
using eu.iamia.NCD.API.Contract;
using eu.iamia.NCD.Bridge;
using eu.iamia.NCD.Shared;
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

        public Register Register1 = new Register(0x01, "Register1", 8);
        public Register Register2 = new Register(0x02, "Register2", 8);

        protected override IEnumerable<Register> Registers => new List<Register>
        {
            Register1,
            Register2
        };

        public DateTime Timestamp { get; set; }
    }

    public partial class BaseModuleShould
    {
        private byte[] ResponseWriteSuccess = { 0x55 };
        private byte[] ResponseError90 = { 0x5A };

        private readonly IBridge Bridge = Substitute.For<IBridge>();

        public BaseModuleTest Sut { get; }

        public BaseModuleShould()
        {
            Sut = new BaseModuleTest(Bridge);
            Bridge.Execute(Arg.Any<ICommand>()).Returns(new NcdApiProtocol(ResponseWriteSuccess));
        }

        [Fact]
        public void ThrowExceptionForNullIBridgeInConstructor()
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
        public void HaveElementsInEnumerator()
        {
            Sut.Register1.SetOutputDirty();
            Sut.Register2.SetOutputDirty();

            var enumerator = Sut.GetEnumerator();

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
            Sut.Register1.SetOutputDirty();
            Sut.Register2.SetOutputDirty();
            
            Sut.Send();
            Bridge.Received(1).Execute(Arg.Any<ICommand>());

            Assert.False(Sut.Register1.IsOutputDirty);
            Assert.False(Sut.Register2.IsOutputDirty);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void CallExecuteForSendWithRetry(int retryCount)
        {
            Sut.Register1.SetOutputDirty();
            Sut.Register2.SetOutputDirty();

            Bridge.Execute(Arg.Any<ICommand>()).Returns(new NcdApiProtocol(ResponseError90));
            
            Sut.RetryCount = retryCount;
            Sut.Send();

            Bridge.Received(Sut.RetryCount + 1).Execute(Arg.Any<ICommand>());
        }
        #endregion

        #region SelectRegisterForReadingWithAutoIncrement

        [Fact]
        public void NotClearIsOutputDirtyForExplisitSendRegister()
        {
            Sut.Register1.SetOutputDirty();

            Bridge.Execute(Arg.Any<ICommand>()).Returns(new NcdApiProtocol(ResponseError90));

            Sut.SelectRegisterForReadingWithAutoIncrement(Sut.Register1);

            Bridge.Received(1).Execute(Arg.Any<ICommand>());
            Assert.True(Sut.Register1.IsOutputDirty);
        }

        #endregion
    }
}