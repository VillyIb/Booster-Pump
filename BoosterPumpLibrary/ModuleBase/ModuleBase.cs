using System;
using EnsureThat;
using eu.iamia.NCD.API.Contract;
using eu.iamia.Util.Extensions;

namespace BoosterPumpLibrary.ModuleBase
{
    public interface IModuleBase
    {
        Guid Id { get; }

        byte DefaultAddress { get; }

        ByteExtension AddressIncrement { get; }

        byte DeviceAddress { get; }

        /// <summary>
        /// Adds the specified value to the DefaultAddress, legal values: {0|1}. // TODO some modules can add up to 7
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public void SetAddressIncrement(int value);

    }

    public abstract class ModuleBase : IModuleBase
    {
        public const int ResponseWriteSuccess = 0x55;

        protected readonly IBridge ApiToSerialBridge;

        public Guid Id { get; }

        public abstract byte DefaultAddress { get; }

        public ByteExtension AddressIncrement { get; protected set; }

        public byte DeviceAddress => DefaultAddress + (AddressIncrement ?? new ByteExtension(0));

        protected ModuleBase(IBridge apiToSerialBridge)
        {
            Ensure.That(apiToSerialBridge, nameof(apiToSerialBridge)).IsNotNull();

            ApiToSerialBridge = apiToSerialBridge;
            AddressIncrement = null;
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