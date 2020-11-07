using System.Collections.Generic;

namespace BoosterPumpLibrary.Commands
{
    public class Test2WayCommand  : CommandBase
    {
        public override IEnumerable<byte> I2C_Data() => new byte[] { 0x21 };
    }
}
