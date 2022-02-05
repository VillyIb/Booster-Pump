using System;
using BoosterPumpLibrary.ModuleBase;
using BoosterPumpLibrary.Util;
using EnsureThat;
using eu.iamia.NCD.API.Contract;
using eu.iamia.Util.Extensions;

namespace eu.iamia.BaseModule
{
    public abstract class ModuleBase : IModuleBase
    {
        public const int ResponseWriteSuccess = 0x55;

        protected readonly IBridge ApiToSerialBridge;

        public Guid Id { get; }

        public abstract byte DefaultAddress { get; }

        public ByteWrapper AddressIncrement { get; protected set; } // TODO right to use here? high coupling :(

        public byte DeviceAddress => DefaultAddress + AddressIncrement;

        protected ModuleBase(IBridge apiToSerialBridge)
        {
            Ensure.That(apiToSerialBridge, nameof(apiToSerialBridge)).IsNotNull();

            ApiToSerialBridge = apiToSerialBridge;
            AddressIncrement = 0;
            Id = Guid.NewGuid();
            Console.WriteLine($"{GetType().Name}: {Id}");
        }

        // TODO NOT generic - valid value range is independent for each module.
        /// <summary>
        /// Adds the specified value to the DefaultAddress, legal values: {0|1}. // TODO some modules can add up to 7
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public void SetAddressIncrement(int value)
        {
            if (value.IsOutsideRange(0, 1))
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, "Valid: {0:1}");
            }

            AddressIncrement = value;
        }


    }
}