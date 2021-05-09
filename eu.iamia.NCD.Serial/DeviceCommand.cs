using System;
using System.Collections.Generic;
using eu.iamia.NCD.API.Contract;
using eu.iamia.NCD.DeviceCommunication.Contract;

namespace eu.iamia.NCD.Serial
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

        internal DeviceRead(ICommand command) : base(command)
        { }
    }

    public class DeviceWrite : DeviceCommand
    {
        public static byte SerialCommandValue => 0xBE;

        protected override byte SerialCommand => SerialCommandValue;

        internal DeviceWrite(ICommand command) : base(command)
        { }
    }

    public class DeviceWriteRead : DeviceCommand
    {
        public static byte SerialCommandValue => 0xC0;

        protected override byte SerialCommand => SerialCommandValue;

        internal DeviceWriteRead(ICommand command) : base(command)
        { }
    }

    public class DeviceBusScan : DeviceCommand
    {
        public static byte SerialCommandValue => 0xC1;

        protected override byte SerialCommand => SerialCommandValue;

        internal DeviceBusScan(ICommand command) : base(command)
        { }
    }

    public class DeviceStopCommand : DeviceCommand
    {
        public static byte SerialCommandValue => 0xFE;

        protected override byte SerialCommand => SerialCommandValue;

        internal DeviceStopCommand(ICommand command) : base(command)
        { }
    }

    public class DeviceConverterHardRebootCommand : DeviceCommand
    {
        public static byte SerialCommandValue => 0xFE;

        protected override byte SerialCommand => SerialCommandValue;

        internal DeviceConverterHardRebootCommand(ICommand command) : base(command)
        { }
    }

    public class DeviceConverterRebootCommand : DeviceCommand
    {
        public static byte SerialCommandValue => 0xFE;

        protected override byte SerialCommand => SerialCommandValue;

        internal DeviceConverterRebootCommand(ICommand command) : base(command)
        { }
    }

    public class DeviceFactory
    {
        public DeviceCommand GetDevice(ICommand command)
        {

            // TODO handle serial device commands.
            return command switch
            {
                ICommandRead _ => new DeviceRead(command),
                ICommandWrite _ => new DeviceWrite(command),
                ICommandWriteRead _ => new DeviceWriteRead(command),
                ICommandControllerBusScan _ => new DeviceBusScan(command),
                ICommandControllerHardReboot _ => new DeviceConverterHardRebootCommand(command),
                ICommandControllerReboot _ => new DeviceConverterRebootCommand(command),
                ICommandControllerStop _ => new DeviceStopCommand(command),
                _ => null
            };
        }

        public INcdApiProtocol GetI2CCommand(ICommand command)
        {
            return new NcdApiProtocol(GetDevice(command).GetDevicePayload());
        }
    }
}
