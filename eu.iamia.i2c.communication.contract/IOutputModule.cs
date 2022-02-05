using System;

namespace eu.iamia.i2c.communication.contract
{
    public interface IOutputModule
    {
        /// <summary>
        /// Default value: 1.
        /// </summary>
        public int RetryCount { get; set; }

        Guid Id { get; }

        byte DefaultAddress { get; }

        byte AddressIncrement { get; }

        byte DeviceAddress { get; }

        public void Send(IOutputModuleEnumerator enumerator);

        public void Send();

        public void SendSpecificRegister(IRegister register);

        public void SetOutputRegistersDirty();

        public void SetAddressIncrement(int value);
    }
}