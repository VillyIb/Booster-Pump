using System.Collections.Generic;
using eu.iamia.NCD.API.Contract;
using eu.iamia.NCD.DeviceCommunication.Contract;

namespace eu.iamia.NCD.API
{
    // TODO rename to ...Factory...

    public abstract class DeviceCommand
    {
        public ICommand Command { get; }

        protected abstract byte SerialCommand { get; }

        protected DeviceCommand(ICommand command)
        {
            Command = command;
        }

        public IEnumerable<byte> GetDevicePayload()
        {
            yield return SerialCommand;
            foreach (var payloadElement in Command.I2C_Data())
            {
                yield return payloadElement;
            }
        }
    }

    public class DeviceRead : DeviceCommand
    {
        public static byte SerialCommandValue => 0xBF;

        protected override byte SerialCommand => SerialCommandValue;

        public DeviceRead(ICommand command) : base(command)
        { }
    }

    public class DeviceWrite : DeviceCommand
    {
        public static byte SerialCommandValue => 0xBE;

        protected override byte SerialCommand => SerialCommandValue;

        public DeviceWrite(ICommand command) : base(command)
        { }
    }

    public class DeviceWriteRead : DeviceCommand
    {
        public static byte SerialCommandValue => 0xC0;

        protected override byte SerialCommand => SerialCommandValue;

        public DeviceWriteRead(ICommand command) : base(command)
        { }
    }

    public class DeviceBusScan : DeviceCommand
    {
        public static byte SerialCommandValue => 0xC1;

        protected override byte SerialCommand => SerialCommandValue;

        public DeviceBusScan(ICommand command) : base(command)
        { }
    }

    public class DeviceStopCommand : DeviceCommand
    {
        public static byte SerialCommandValue => 0xFE;

        protected override byte SerialCommand => SerialCommandValue;

        public DeviceStopCommand(ICommand command) : base(command)
        { }
    }

    public class DeviceConverterHardRebootCommand : DeviceCommand
    {
        public static byte SerialCommandValue => 0xFE;

        protected override byte SerialCommand => SerialCommandValue;

        public DeviceConverterHardRebootCommand(ICommand command) : base(command)
        { }
    }

    public class DeviceConverterRebootCommand : DeviceCommand
    {
        public static byte SerialCommandValue => 0xFE;

        protected override byte SerialCommand => SerialCommandValue;

        public DeviceConverterRebootCommand(ICommand command) : base(command)
        { }
    }

    public class DeviceFactory
    {
       

       
    }
}
