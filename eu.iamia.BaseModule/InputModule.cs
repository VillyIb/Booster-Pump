#nullable enable
using System.Linq;
using eu.iamia.i2c.communication.contract;
using eu.iamia.NCD.API;
using eu.iamia.NCD.API.Contract;

namespace eu.iamia.BaseModule
{
    // TODO create OutputInputModule implement Send+Read on same register.

    public abstract class InputModule : OutputModule, IInputModule
    {
        protected InputModule(IBridge apiToSerialBridge) : base(apiToSerialBridge)
        { }

        //public abstract void ReadFromDevice();

        public abstract bool IsInputValid { get; }

      

        private InputModuleEnumerator GetInputEnumerator()
        {
            var registersToSend = Registers.Where(register => register.IsInput && register.IsInputDirty);
            return new InputModuleEnumerator(registersToSend, DeviceAddress);
        }

        public virtual void ReadFromDevice()
        {
            using var register = GetInputEnumerator();
            var currentRetryCount = RetryCount;
            while (register.MoveNext() && register.Current != null && currentRetryCount > 0)
            {
                var lengthRequested = (byte)register.Current.Size;
                ICommand command = new CommandRead(register.Current.RegisterAddress, lengthRequested);

                var response = ApiToSerialBridge.Execute(command);

                if (
                    response.IsValid
                    &&
                    !response.IsError
                    &&
                    response.ByteCount == lengthRequested
                )
                {
                    register.Current.Value = response.Value;
                    continue;
                }
                register.Reset();
                currentRetryCount--;
            }
        }

        public void SetInputRegistersDirty()
        {
            foreach (var register in Registers)
            {
                if (!register.IsInput) { continue; }

                register.IsInputDirty = true;
            }
        }
    }
}
