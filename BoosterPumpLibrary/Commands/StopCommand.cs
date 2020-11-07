using System.Collections.Generic;

namespace BoosterPumpLibrary.Commands
{
    public class StopCommand : CommandBase
    {
        public override IEnumerable<byte> I2C_Data() => new byte[0];
    }
}
