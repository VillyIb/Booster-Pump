using System;

namespace eu.iamia.i2c.communication.contract
{
    public interface IModuleBase
    {
        Guid Id { get; }

        byte DefaultAddress { get; }

        byte AddressIncrement { get; }

        byte DeviceAddress { get; }

        /// <summary>
        /// Adds the specified value to the DefaultAddress, legal values: {0|1}. // TODO some modules can add up to 7
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public void SetAddressIncrement(int value);

    }
}