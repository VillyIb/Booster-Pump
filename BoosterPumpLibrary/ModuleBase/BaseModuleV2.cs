using BoosterPumpLibrary.Commands;
using BoosterPumpLibrary.Contracts;
using BoosterPumpLibrary.Settings;
using System.Collections.Generic;
using System.Linq;

namespace BoosterPumpLibrary.ModuleBase
{
    public abstract partial class BaseModuleV2
    {
        public abstract byte DefaultAddress { get; }

        public ByteWrapper AddressIncrement { get; protected set; }

        public byte DeviceAddress => DefaultAddress + AddressIncrement;

        protected ISerialConverter SerialPort { get; }

        public abstract void Init();

        protected BaseModuleV2(ISerialConverter serialPort, int? addressIncrement = 0)
        {
            SerialPort = serialPort;
            AddressIncrement = addressIncrement ?? 0;
        }

        protected abstract IEnumerable<RegisterBase> Registers { get; }

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
                if(retryCount > 0 && (fromDevice.Payload.Length != 1  || fromDevice.Payload[0] != 55))
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
