using System.Collections.Generic;

namespace BoosterPumpLibrary.Commands
{
    public class ReadCommand : CommandBase
    {
        public byte LengthRequested { get; set; }

        public override IEnumerable<byte> I2C_Data()
        {
            yield return DeviceAddress;
            yield return LengthRequested;
        }
    }
}
