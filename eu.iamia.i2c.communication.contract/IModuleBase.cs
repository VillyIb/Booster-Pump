using System;
using System.Collections.Generic;
// ReSharper disable UnusedMemberInSuper.Global

namespace eu.iamia.i2c.communication.contract
{
    public interface IModuleBase
    {
        Guid Id { get; }

        byte DeviceAddress { get; }

        public IEnumerable<IRegister> Registers { get; }

        void SetupOnlyOnce(IEnumerable<IRegister> registers, byte deviceAddress);

    }
}