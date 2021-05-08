using System;
using System.Collections.Generic;
using eu.iamia.NCD.API;
using eu.iamia.NCD.API.Contract;
using eu.iamia.NCD.DeviceCommunication.Contract;

namespace eu.iamia.NCD.Serial
{

    // TODO Not DeviceCommand -> ...GatewayCommand
    public abstract class DeviceCommand
    {
        public ICommand Command { get; }

        public abstract byte SerialCommand { get; }

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
        public override byte SerialCommand => 0xBF;

        public DeviceRead(ICommand command) : base(command)
        { }
    }

    public class DeviceWrite : DeviceCommand
    {
        public override byte SerialCommand => 0xBE;

        public DeviceWrite(ICommand command) : base(command)
        { }
    }

    public class DeviceWriteRead : DeviceCommand
    {
        public override byte SerialCommand => 0xC0;

        public DeviceWriteRead(ICommand command) : base(command)
        { }
    }

    public class DeviceConverterCommand : DeviceCommand
    {
        public override byte SerialCommand => 0xFE;

        public DeviceConverterCommand(ICommand command) : base(command)
        { }
    }

    public class DeviceBusScan : DeviceConverterCommand
    {
        public override byte SerialCommand => 0xC1;

        public DeviceBusScan(ICommand command) : base(command)
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
                ICommandControllerHardReboot _ => new DeviceConverterCommand(command),
                ICommandControllerReboot _ => new DeviceConverterCommand(command),
                ICommandControllerStop _ => new DeviceConverterCommand(command),

                _ => null
            };
        }



        public DeviceCommand GetDevice(ICommandRead command) => new DeviceRead(command);
        public DeviceCommand GetDevice(ICommandWrite command) => new DeviceWrite(command);
        public DeviceCommand GetDevice(ICommandWriteRead command) => new DeviceWriteRead(command);

        public DeviceCommand GetDevice(ICommandControllerBusScan command) => new DeviceBusScan(command);
        public DeviceCommand GetDevice(ICommandControllerHardReboot command) => new DeviceConverterCommand(command);
        public DeviceCommand GetDevice(ICommandControllerReboot command) => new DeviceConverterCommand(command);
        public DeviceCommand GetDevice(ICommandControllerStop command) => new DeviceConverterCommand(command);

    }
}
