using System.Collections.Generic;
using System.Linq;
using BoosterPumpLibrary.ModuleBase;
using BoosterPumpLibrary.Settings;
using eu.iamia.NCD.API.Contract;

// ReSharper disable UnusedMember.Global

namespace eu.iamia.BaseModule
{
    public abstract partial class OutputModule : ModuleBase, IOutputModule
    {
        /// <summary>
        /// Default value: 1.
        /// </summary>
        public int RetryCount { get; set; } = 1;

        protected OutputModule(IBridge apiToSerialBridge) : base(apiToSerialBridge)
        { }

        protected abstract IEnumerable<Register> Registers { get; }

        private IOutputModuleEnumerator GetOutputEnumerator()
        {
            var registersToSend = Registers.Where(register => register.IsOutput && register.IsOutputDirty);
            return new OutputModuleEnumerator(registersToSend, DeviceAddress);
        }

        public void Send(IOutputModuleEnumerator enumerator)
        {
            var currentRetryCount = RetryCount;
            while (enumerator.MoveNext() && enumerator.Current != null)
            {
                var fromDevice = ApiToSerialBridge.Execute(enumerator.Current);

                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (
                    currentRetryCount <= 0
                    ||
                    fromDevice.Payload.Count == 1
                    &&
                    fromDevice.Payload[0] == ResponseWriteSuccess)
                {
                    continue;
                }

                enumerator.Reset();
                currentRetryCount--;
            }
        }

        public void Send()
        {
            using var enumerator = GetOutputEnumerator();
            Send(enumerator);
        }

        public void SendSpecificRegister(Register register)
        {
            var registersToSend = new[] { register };
            using var enumerator = new OutputModuleEnumerator(registersToSend, DeviceAddress);
            Send(enumerator);
        }

        public void SetOutputRegistersDirty()
        {
            foreach (var register in Registers)
            {
                if (!register.IsOutput)
                {
                    continue;
                }

                register.IsOutputDirty = true;
            }
        }
    }
}
