using System;

namespace eu.iamia.i2c.communication.contract
{
    public interface IInputModule
    {
        bool IsInputValid { get; }

        Guid Id { get; }

        byte DefaultAddress { get; }

        byte AddressIncrement { get; }

        byte DeviceAddress { get; }

        /// <summary>
        /// Default value: 1.
        /// </summary>
        int RetryCount { get; set; }

        void ReadFromDevice();

        void SetInputRegistersDirty();

        void SetAddressIncrement(int value);
    }
}