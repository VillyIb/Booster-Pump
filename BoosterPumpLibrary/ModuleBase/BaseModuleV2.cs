using System;
using BoosterPumpLibrary.Commands;
using BoosterPumpLibrary.Contracts;
using BoosterPumpLibrary.Settings;
using System.Collections.Generic;
using System.Linq;

namespace BoosterPumpLibrary.ModuleBase
{
    public abstract partial class BaseModuleV2
    {
        public Guid Id { get; }
        public abstract byte DefaultAddress { get; }

        public ByteWrapper AddressIncrement { get; protected set; }

        public byte DeviceAddress => DefaultAddress + (AddressIncrement ?? new ByteWrapper(0));

        protected ISerialConverter SerialPort { get; }

        public abstract void Init();

        protected BaseModuleV2(ISerialConverter serialPort)
        {
            SerialPort = serialPort;
            AddressIncrement = null;
            Id = Guid.NewGuid();
        }

        // TODO NOT generic - valid value range is independent for each module.
        /// <summary>
        /// Adds the specified value to the DefaultAddress, legal values: {0|1}.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public void SetAddressIncrement(int value)
        {
            if (value < 0 | 1 < value) { throw new ArgumentOutOfRangeException(nameof(value), value, "Valid: {0:1}"); }
            AddressIncrement = value;            
        }

        protected abstract IEnumerable<RegisterBase> Registers
        { get; }

        public ModuleEnumerator GetEnumerator()
        {
            return new ModuleEnumerator(Registers.Where(t => t.IsDirty), DeviceAddress);
        }

        public void Send()
        {
            using var enumerator = GetEnumerator();
            var retryCount = 0;
            while (enumerator.MoveNext())
            {
                var command = enumerator.Current;
                var fromDevice = SerialPort.Execute(command);

                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (retryCount > 0 && (fromDevice.Payload.Length != 1 || fromDevice.Payload[0] != 55))
                {
                    enumerator.Reset();
                    retryCount--;
                }
            }
        }

        public void SelectRegisterForReading(Register register)
        {
            var writeCommand = new WriteCommand { DeviceAddress = DeviceAddress, Payload = new[] { register.RegisterAddress } };
            // ReSharper disable once UnusedVariable
            var returnValue = SerialPort.Execute(writeCommand);
        }

        public void SelectRegisterForReadingWithAutoIncrement(Register register)
        {
            var writeCommand = new WriteCommand { DeviceAddress = DeviceAddress, Payload = new[] { (byte)(register.RegisterAddress | 0x80) } };
            // ReSharper disable once UnusedVariable
            var returnValue = SerialPort.Execute(writeCommand);
        }

    }
}
