using System;
using System.Collections.Generic;
// ReSharper disable UnusedMemberInSuper.Global

namespace eu.iamia.i2c.communication.contract
{
    public interface IInputModule
    {
        bool IsInputValid { get; }

        Guid Id { get; }

        byte DeviceAddress { get; }

        /// <summary>
        /// Default value: 1.
        /// </summary>
        int RetryCount { get; set; }

        void ReadFromDevice();

        void SetInputRegistersDirty();

        void SetupOnlyOnce(IEnumerable<IRegister> registers, byte deviceAddress);

        // TODO in separate interface for both input and output 
        void Send();

        void SendSpecificRegister(IRegister register);
    }
}