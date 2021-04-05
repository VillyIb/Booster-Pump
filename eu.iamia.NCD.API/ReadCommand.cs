using System.Collections.Generic;
using eu.iamia.NCD.API.Contract;

namespace eu.iamia.NCD.API
{
    public class ReadCommand : CommandBase, INcdApiCommand
    {
        public byte LengthRequested { get; set; }

        public ReadCommand(byte deviceAddress, byte lengthRequested)
            : base(deviceAddress)
        {
            LengthRequested = lengthRequested;
        }

        public override IEnumerable<byte> I2C_Data()
        {
            yield return DeviceAddress;
            yield return LengthRequested;
        }
    }
}
