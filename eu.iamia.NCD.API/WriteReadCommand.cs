using System.Collections.Generic;
using eu.iamia.NCD.API.Contract;

namespace eu.iamia.NCD.API
{
    public class WriteReadCommand : CommandBase, INcdApiCommand
    {
        public IEnumerable<byte> Payload { get; set; }

        public byte LengthRequested { get; set; }

        public byte Delay { get; set; }

        public WriteReadCommand(byte deviceAddress, IEnumerable<byte> payload, byte lengthRequested, byte delay = 0x16)
        : base(deviceAddress)
        {
            Payload = payload;
            LengthRequested = lengthRequested;
            Delay = delay;
        }

        public override IEnumerable<byte> I2C_Data()
        {
            yield return DeviceAddress;
            yield return LengthRequested;
            yield return Delay;
            foreach (var current in Payload)
            {
                yield return current;
            }
        }
    }
}
