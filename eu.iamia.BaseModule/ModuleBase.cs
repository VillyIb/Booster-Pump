using System;
using System.Collections.Generic;
using System.Linq;
using EnsureThat;
using eu.iamia.i2c.communication.contract;
using eu.iamia.NCD.API.Contract;

namespace eu.iamia.BaseModule
{
    public abstract class ModuleBase : IModuleBase
    {
        public const int ResponseWriteSuccess = 0x55;

        protected readonly IBridge ApiToSerialBridge;

        public Guid Id { get; }

        public byte DeviceAddress { get; private set; }

        public IEnumerable<IRegister> Registers { get; private set; }

        public void SetupOnlyOnce(IEnumerable<IRegister> registers, byte deviceAddress)
        {
            if (!Registers.Any())
            {
                throw new ArgumentOutOfRangeException(nameof(registers), "Collection can not be empty");
            }

            if (Registers.Any())
            {
                throw new InvalidOperationException("Setup can only be called once.");
            }

            Registers = registers;
            DeviceAddress = deviceAddress;
        }

        protected ModuleBase(IBridge apiToSerialBridge)
        {
            Ensure.That(apiToSerialBridge, nameof(apiToSerialBridge)).IsNotNull();

            Registers = new List<IRegister>();
            ApiToSerialBridge = apiToSerialBridge;
            Id = Guid.NewGuid();
            Console.WriteLine($"{GetType().Name}: {Id}");
        }
    }
}