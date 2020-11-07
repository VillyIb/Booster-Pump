using System.Collections.Generic;

namespace BoosterPumpLibrary.Commands
{
    public class WriteCommand : CommandBase
    {
        public IEnumerable<byte> Payload { get; set; }

        public override IEnumerable<byte> I2C_Data()
        {
            yield return Address;
            foreach (var current in Payload)
            {
                yield return current;
            }
        }
    }
}
