using System;
using System.Collections.Generic;
using System.Linq;
using BoosterPumpLibrary.Settings;
using eu.iamia.NCD.API.Contract;
using eu.iamia.Util.Extensions;

// ReSharper disable UnusedMember.Global

namespace BoosterPumpLibrary.ModuleBase
{
    public interface IOutputModule
    {
        /// <summary>
        /// Default value: 1.
        /// </summary>
        public int RetryCount { get; set; }

        Guid Id { get; }

        byte DefaultAddress { get; }

        ByteExtension AddressIncrement { get; }

        byte DeviceAddress { get; }

        public void Send(OutputModule.OutputModuleEnumerator enumerator);

        public void Send();

        public void SendSpecificRegister(Register register);

        public void SetOutputRegistersDirty();

        public void SetAddressIncrement(int value);
    }

    public abstract partial class OutputModule : ModuleBase, IOutputModule
    {
        /// <summary>
        /// Default value: 1.
        /// </summary>
        public int RetryCount { get; set; } = 1;

        protected OutputModule(IBridge apiToSerialBridge) : base(apiToSerialBridge)
        { }

        protected abstract IEnumerable<Register> Registers { get; }

        private OutputModuleEnumerator GetOutputEnumerator()
        {
            var registersToSend = Registers.Where(register => register.IsOutput && register.IsOutputDirty);
            return new(registersToSend, DeviceAddress);
        }

        public void Send(OutputModuleEnumerator enumerator)
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
