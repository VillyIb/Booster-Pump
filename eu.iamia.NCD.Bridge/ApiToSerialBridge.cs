using eu.iamia.NCD.API;
using eu.iamia.NCD.API.Contract;
using eu.iamia.NCD.Serial.Contract;
using eu.iamia.NCD.Shared;

namespace eu.iamia.NCD.Bridge
{
    /// <summary>
    /// Translates ICommand to 
    /// </summary>
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
                ICommandRead => new DeviceRead(command),
                ICommandWrite => new DeviceWrite(command),
                ICommandWriteRead => new DeviceWriteRead(command),
                ICommandControllerBusScan => new DeviceBusScan(command),
                ICommandControllerHardReboot => new DeviceConverterCommand(command),
                ICommandControllerReboot => new DeviceConverterCommand(command),
                ICommandControllerStop => new DeviceConverterCommand(command),
                _ => null
            };
        }

        internal INcdApiProtocol GetI2CCommand(ICommand command)
        {
            return new NcdApiProtocol(GetDevice(command).GetDevicePayload());
        }

        public IDataFromDevice Execute(ICommand command)
        {
            var dc = GetDevice(command);
            var i2CCommand = new NcdApiProtocol(dc.GetDevicePayload());

            var i2CResponse = Gateway.Execute(i2CCommand);
            if (i2CResponse is null)
            {
                return null;

            }

            return new DataFromDevice(i2CResponse.Header, i2CResponse.ByteCount, i2CResponse.Payload, i2CResponse.Checksum);
        }

        public void Dispose()
        {
            Gateway?.Dispose();
        }
    }
}
