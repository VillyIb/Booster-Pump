using BoosterPumpLibrary.Commands;
using BoosterPumpLibrary.Contracts;
using System.Collections.Generic;
using System.Linq;

namespace BoosterPumpLibrary.ModuleBase
{
    public class ByteWrapper
    {
        private byte payload;

        public ByteWrapper(int value)
        {
            payload = (byte)value;
        }

        public static ByteWrapper operator +(ByteWrapper first, ByteWrapper second)
        {
            return new ByteWrapper(first.payload + second.payload);
        }

        public static ByteWrapper operator +(ByteWrapper first, byte second)
        {
            return new ByteWrapper(first.payload + second);
        }

        public static implicit operator byte(ByteWrapper value)
        {
            return value.payload;
        }

        public static implicit operator ByteWrapper(int value)
        {
            return new ByteWrapper(value);
        }
    }


    public abstract class BaseModule
    {

        public abstract byte DefaultAddress { get; }

        public ByteWrapper AddressIncrement { get; set; }

        public byte Address => DefaultAddress + AddressIncrement;

        protected ISerialConverter SerialPort { get; }

        public abstract void Init();

        public BaseModule(ISerialConverter serialPort, int? addressIncrement = 0)
        {
            SerialPort = serialPort;            
            AddressIncrement = null != addressIncrement ? addressIncrement.Value : 0;
        }

        protected abstract IEnumerable<Register> Registers { get; }

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

                if (result.Count > 0 && currentRegisterId + 1 != current.RegisterId) { break; }
                if (result.Count == 0)
                {
                    result.Add(current.RegisterId);
                }
                result.Add(current.GetDataRegisterAndClearDirty());
                currentRegisterId = current.RegisterId;
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

        public void SelectRegisterForReading(Register register)
        {
            var writeCommand = new WriteCommand { Address = Address, Payload = new[] { register.RegisterId } };
            var returnValue = SerialPort.Execute(writeCommand);
        }

        public void SelectRegisterForReadingWithAutoIncrement(Register register)
        {
            var writeCommand = new WriteCommand { Address = Address, Payload = new[] { (byte)(register.RegisterId | 0x80) } };
            var returnValue = SerialPort.Execute(writeCommand);
        }

    }
}
