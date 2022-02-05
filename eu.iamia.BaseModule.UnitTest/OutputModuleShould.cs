using System;
using System.Collections.Generic;
using BoosterPumpLibrary.ModuleBase;
using BoosterPumpLibrary.Settings;
using eu.iamia.NCD.API.Contract;
using eu.iamia.NCD.Shared;
using NSubstitute;
using Xunit;

namespace eu.iamia.BaseModule.UnitTest
{
    public class OutputModuleTest : OutputModule
    {
        public OutputModuleTest(IBridge apiToSerialBridge, byte register2Offset) : base(apiToSerialBridge)
        {
            Timestamp = DateTime.Now;
            Register2Out.RegisterAddress += register2Offset;
        }

        public override byte DefaultAddress => 0b00;

        public Register Register1Out = new Register(0x01, "Register1Out", 8, Direction.Output);
        public Register Register2Out = new Register(0x02, "Register2Out", 8, Direction.Output);
        public Register Register3In = new Register(0x03, "Register3In", 8, Direction.Input);

        protected override IEnumerable<Register> Registers => new List<Register>
        {
            Register1Out,
            Register2Out,
            Register3In
        };

        public DateTime Timestamp { get; set; }
    }

    public class OutputModuleShould
    {
        private readonly IBridge Bridge = Substitute.For<IBridge>();

        public IOutputModule Sut { get; private set;}

        public OutputModuleShould()
        {
            Sut = new OutputModuleTest(Bridge, 0);
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

        [Fact]
        public void CallExecute2TimesForSendWithNonConsecutiveRegisters()
        {
            Sut = new OutputModuleTest(Bridge, 1);
            Sut.SetOutputRegistersDirty();
            ;
            Sut.Send();
            Bridge.Received(2).Execute(Arg.Any<ICommand>());
        }

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
            ((OutputModuleTest)Sut).Register1Out.IsOutputDirty = true;

            Bridge.Execute(Arg.Any<ICommand>()).Returns(NcdApiProtocol.WriteSuccess);

            Sut.SendSpecificRegister(((OutputModuleTest)Sut).Register1Out);

            Bridge.Received(1).Execute(Arg.Any<ICommand>());
        }

        #endregion
    }
}
