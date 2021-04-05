using BoosterPumpLibrary.Commands;
using NCD_API_SerialConverter.NcdApiProtocol.DeviceCommands;

namespace NCD_API_SerialConverter
{
    public class DeviceFactory
    {
        public IDevice GetDevice(ReadCommand command)
        {
            return new DeviceRead(command);
        }
        public IDevice GetDevice(WriteCommand command)
        {
            return new DeviceWrite(command);
        }
        public IDevice GetDevice(WriteReadCommand command)
        {
            return new DeviceWriteRead(command);
        }
    }
}
