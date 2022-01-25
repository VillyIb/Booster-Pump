using System;
using System.Collections.Generic;
using System.Linq;
using BoosterPumpLibrary.Settings;
using EnsureThat;
using eu.iamia.NCD.API;
using eu.iamia.NCD.API.Contract;
using eu.iamia.Util.Extensions;

// ReSharper disable UnusedMember.Global

namespace BoosterPumpLibrary.ModuleBase
{
    public abstract partial class OutputModule
    {
        public const int ResponseWriteSuccess = 0x55;

        protected readonly IBridge ApiToSerialBridge;

        public Guid Id { get; }

        public abstract byte DefaultAddress { get; }

        public ByteExtension AddressIncrement { get; protected set; }

        public byte DeviceAddress => DefaultAddress + (AddressIncrement ?? new ByteExtension(0));

        /// <summary>
        /// Default value: 1.
        /// </summary>
        public int RetryCount { get; set; } = 1;

        protected OutputModule(IBridge apiToSerialBridge)
        {
            Ensure.That(apiToSerialBridge, nameof(apiToSerialBridge)).IsNotNull();

            ApiToSerialBridge = apiToSerialBridge;
            AddressIncrement = null;
            Id = Guid.NewGuid();
            Console.WriteLine($"{GetType().Name}: {Id}");
        }

        // TODO NOT generic - valid value range is independent for each module.
        /// <summary>
        /// Adds the specified value to the DefaultAddress, legal values: {0|1}.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public void SetAddressIncrement(int value)
        {
            if (value.IsOutsideRange(0, 1)) { throw new ArgumentOutOfRangeException(nameof(value), value, "Valid: {0:1}"); }
            AddressIncrement = value;
        }

        protected abstract IEnumerable<Register> Registers { get; }

        public virtual OutputModuleEnumerator GetOutputEnumerator()
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
    }
}
