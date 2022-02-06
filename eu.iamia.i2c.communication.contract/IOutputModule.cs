using System;
using System.Collections.Generic;

namespace eu.iamia.i2c.communication.contract
{
    public interface IOutputModule
    {
        /// <summary>
        /// Default value: 1.
        /// </summary>
        int RetryCount { get; set; }

        Guid Id { get; }

        byte DeviceAddress { get; }

        void Send();

        void SendSpecificRegister(IRegister register);

        void SetOutputRegistersDirty();

        void SetupOnlyOnce(IEnumerable<IRegister> registers, byte deviceAddress);

    }
}