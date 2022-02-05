using System;
using System.Collections.Generic;
using BoosterPumpLibrary.Settings;
using eu.iamia.i2c.communication.contract;
using eu.iamia.NCD.API.Contract;
using NSubstitute;
using Xunit;

namespace eu.iamia.BaseModule.UnitTest
{
    public class InputModuleTest : InputModule
    {
        public override byte DefaultAddress => 0x00;

        public Register Register1In = new Register(0x01, "Register1Out", 8, Direction.Input);
        public Register Register2In = new Register(0x02, "Register2Out", 8, Direction.Input);
        public Register Register3Out = new Register(0x03, "Register2Out", 8, Direction.Output);

        protected override IEnumerable<Register> Registers => new List<Register>
        {
            Register1In,
            Register2In
        };

        public override bool IsInputValid { get; }

        public InputModuleTest(IBridge apiToSerialBridge) : base(apiToSerialBridge)
        {
        }

    }

    public class InputModuleShould
    {
        public IInputModule Sut;

        private readonly IBridge Bridge = Substitute.For<IBridge>();

        public InputModuleShould()
        {
            Sut = new InputModuleTest(Bridge);
        }

        [Fact]
        public void ThrowExceptionForNullIBridgeInConstructor()
        {
            Assert.Throws<ArgumentNullException>(() => new InputModuleTest(null));
        }

        #region IsInputValid

        #endregion

        #region ReadFromDevice

        #endregion

        #region SetInputRegisterDirty

        #endregion


    }
}
