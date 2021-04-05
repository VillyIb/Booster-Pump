using System.Collections.Generic;
using eu.iamia.NCD.API.Contract;

namespace eu.iamia.NCD.API
{
    public class WriteCommand : CommandBase, INcdApiCommand
    {
        public IEnumerable<byte> Payload { get; set; }

        public WriteCommand(byte deviceAddress, IEnumerable<byte> payload)
            : base(deviceAddress)
        {
            Payload = payload;
        }

        public override IEnumerable<byte> I2C_Data()
        {
            yield return DeviceAddress;
            foreach (var current in Payload)
            {
                yield return current;
            }
        }
    }
}
