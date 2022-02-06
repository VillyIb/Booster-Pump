using System;
using System.Collections.Generic;
using BoosterPumpLibrary.Settings;
using eu.iamia.BaseModule.Contract;
using eu.iamia.i2c.communication.contract;
using eu.iamia.NCD.API.Contract;
using eu.iamia.NCD.Shared;
using NSubstitute;
using Xunit;

namespace eu.iamia.BaseModule.UnitTest
{
    public class OutputModuleShould
    {
        public byte DefaultAddress => 0b00;

        public Register Register1Out = new Register(0x01, "Register1Out", 8, Direction.Output);
        public Register Register2Out = new Register(0x02, "Register2Out", 8, Direction.Output);
        public Register Register3In = new Register(0x03, "Register3In", 8, Direction.Input);

        protected IEnumerable<Register> Registers => new List<Register>
        {
            Register1Out,
            Register2Out,
            Register3In
        };

        private readonly IBridge Bridge = Substitute.For<IBridge>();

        public IOutputModule Sut { get; private set; }

        public OutputModuleShould()
        {
            Sut = new OutputModule(Bridge);
            Sut.SetupOnlyOnce(Registers, 0x00);
            Bridge.Execute(Arg.Any<ICommand>()).Returns(NcdApiProtocol.WriteSuccess);
        }

        #region Send

        [Fact]
        public void DoNotCallExecuteForSendWithNonDirtyRegisters()
        {
            Sut.Send();
            Bridge.Received(0).Execute(Arg.Any<ICommand>());
        }

        [Fact]
        public void CallExecute1TimeForSendWithConsecutiveRegisters()
        {
            Sut.SetOutputRegistersDirty();
            Sut.Send();
            Bridge.Received(1).Execute(Arg.Any<ICommand>());
        }

        //[Fact]
        //public void CallExecute2TimesForSendWithNonConsecutiveRegisters()
        //{
        //    Sut = new OutputModuleTest(Bridge, 1);
        //    Sut.SetOutputRegistersDirty();
        //    ;
        //    Sut.Send();
        //    Bridge.Received(2).Execute(Arg.Any<ICommand>());
        //}

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void CallExecuteMultipleTimesForSendWithTimeout(int retryCount)
        {
            Sut.SetOutputRegistersDirty();
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
           Register1Out.IsOutputDirty = true;

            Bridge.Execute(Arg.Any<ICommand>()).Returns(NcdApiProtocol.WriteSuccess);

            Sut.SendSpecificRegister(Register1Out);

            Bridge.Received(1).Execute(Arg.Any<ICommand>());
        }

        #endregion
    }
}
