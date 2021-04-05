using System.Collections.Generic;
using eu.iamia.NCD.API;
using eu.iamia.NCD.API.Contract;

namespace eu.iamia.NCD.Serial
{
    public interface IDevice
    {
        INcdApiCommand NcdApiCommand { get; }
        byte SerialCommand { get; }
    }

    public abstract class Device : IDevice
    {
        public INcdApiCommand NcdApiCommand { get; }

        public abstract byte SerialCommand { get; }

        protected Device(INcdApiCommand ncdApiCommand)
        {
            NcdApiCommand = ncdApiCommand;
        }

        public IEnumerable<byte> GetDevicePayload()
        {
            yield return SerialCommand;
            foreach (var payloadElement in NcdApiCommand.I2C_Data())
            {
                yield return payloadElement;
            }
        }
    }

    public class DeviceRead : Device
    {
        public override byte SerialCommand => 0xBF;

        internal DeviceRead(INcdApiCommand ncdApiCommand) : base(ncdApiCommand)
        { }
    }

    public class DeviceWrite : Device
    {
        public override byte SerialCommand => 0xBE;

        internal DeviceWrite(INcdApiCommand ncdApiCommand) : base(ncdApiCommand)
        { }
    }

    public class DeviceWriteRead : Device
    {
        public override byte SerialCommand => 0xC0;

        internal DeviceWriteRead(INcdApiCommand ncdApiCommand) : base(ncdApiCommand)
        { }
    }

    public class DeviceFactory{

        public Device GetDevice(ReadCommand ncdApiCommand) => new DeviceRead(ncdApiCommand);

        public Device GetDevice(WriteCommand ncdApiCommand) => new DeviceWrite(ncdApiCommand);

        public Device GetDevice(WriteReadCommand ncdApiCommand) => new DeviceWriteRead(ncdApiCommand);
    }
}
