using System.Collections.Generic;

namespace BoosterPumpLibrary.Commands
{
    public class WriteReadCommand : CommandBase
    {
        public IEnumerable<byte> Payload { get; set; }

        public byte LengthRequested { get; set; }

        public byte Delay { get; set; }

        public override IEnumerable<byte> I2C_Data()
        {
            yield return Address;
            yield return LengthRequested;
            yield return Delay;
            foreach (var current in Payload)
            {
                yield return current;
            }
        }
    }
}
