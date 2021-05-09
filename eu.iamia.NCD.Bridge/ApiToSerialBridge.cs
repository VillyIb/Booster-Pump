using eu.iamia.NCD.API;
using eu.iamia.NCD.API.Contract;
using eu.iamia.NCD.DeviceCommunication.Contract;
using eu.iamia.NCD.Serial;

namespace eu.iamia.NCD.Bridge
{
    /// <summary>
    /// Translates ICommand to 
    /// </summary>
    // todo implement IDisposeable
    public class ApiToSerialBridge : IBridge
    {
        private readonly IGateway Gateway;

        public ApiToSerialBridge(IGateway gateway)
        {
            Gateway = gateway;
        }

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


        public IDataFromDevice Execute(ICommand command)
        {
            var dc = GetDevice(command);
            var i2CCommand = new NcdApiProtocol(dc.GetDevicePayload());

            var i2CResponse = Gateway.Execute(i2CCommand);

            return new DataFromDevice(i2CResponse.Header, i2CResponse.ByteCount, i2CResponse.Payload, i2CResponse.Checksum);
        }
    }
}
