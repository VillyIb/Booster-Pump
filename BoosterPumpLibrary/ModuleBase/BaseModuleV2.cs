using BoosterPumpLibrary.Commands;
using BoosterPumpLibrary.Contracts;
using BoosterPumpLibrary.Settings;
using System.Collections.Generic;
using System.Linq;

namespace BoosterPumpLibrary.ModuleBase
{ 
    public abstract class BaseModuleV2
    {
        public abstract byte DefaultAddress { get; }

        public ByteWrapper AddressIncrement { get; protected set; }

        public byte Address => DefaultAddress + AddressIncrement;

        protected ISerialConverter SerialPort { get; }

        public abstract void Init();

        protected BaseModuleV2(ISerialConverter serialPort, int? addressIncrement = 0)
        {
            SerialPort = serialPort;            
            AddressIncrement = addressIncrement ?? 0;
        }

        protected abstract IEnumerable<RegisterBase> Registers { get; }

        /// <summary>
        /// Returns next command for each call.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<byte> CurrentCommand()
        {
            var result = new List<byte>();
            byte currentRegisterId = 0;

            foreach (var current in Registers)
            {
                if (!current.IsDirty) continue;

                if (result.Count > 0 && currentRegisterId + 1 != current.RegisterIndex) { break; }
                if (result.Count == 0)
                {
                    result.Add(current.RegisterIndex);
                }
                result.AddRange(current.GetByteValue()); 
                currentRegisterId = current.RegisterIndex;
            }

            return result;
        }

        public bool MoveNextCommand()
        {
            return Registers.Any(t => t.IsDirty);
        }

        public void Send()
        {
            while (MoveNextCommand())
            {
                var output = new List<byte> { DefaultAddress };
                var currentCommand = CurrentCommand().ToList();
                output.AddRange(currentCommand);

                var writeCommand = new WriteCommand { Address = Address, Payload = currentCommand };

                // ReSharper disable once UnusedVariable
                var returnValue = SerialPort.Execute(writeCommand);
            }
        }

        public void SelectRegisterForReading(Settings.Register register)
        {
            var writeCommand = new WriteCommand { Address = Address, Payload = new[] { register.RegisterIndex } };
            // ReSharper disable once UnusedVariable
            var returnValue = SerialPort.Execute(writeCommand);
        }

        public void SelectRegisterForReadingWithAutoIncrement(Settings.Register register)
        {
            var writeCommand = new WriteCommand { Address = Address, Payload = new[] { (byte)(register.RegisterIndex | 0x80) } };
            // ReSharper disable once UnusedVariable
            var returnValue = SerialPort.Execute(writeCommand);
        }

    }
}
