using System.Collections.Generic;
using BoosterPumpLibrary.Settings;
using eu.iamia.BaseModule.Contract;
using eu.iamia.i2c.communication.contract;
using NSubstitute;

namespace eu.iamia.BaseModule.UnitTest
{
    public class InputModuleShould
    {
        public IInputModule Sut;

        private readonly IBridge Bridge = Substitute.For<IBridge>();

        public Register Register1In = new Register(0x01, "Register1Out", 8, Direction.Input);
        public Register Register2In = new Register(0x02, "Register2Out", 8, Direction.Input);
        public Register Register3Out = new Register(0x03, "Register2Out", 8, Direction.Output);

        protected IEnumerable<Register> Registers => new List<Register>
        {
            Register1In,
            Register2In
        };

        public InputModuleShould()
        {
            Sut = new InputModule(Bridge);
            Sut.SetupOnlyOnce(Registers, 0x00);
        }

        #region IsInputValid

        #endregion

        #region ReadFromDevice

        #endregion

        #region SetInputRegisterDirty

        #endregion


    }
}
