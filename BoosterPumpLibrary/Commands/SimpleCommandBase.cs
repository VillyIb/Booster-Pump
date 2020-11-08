using System;
using System.Collections.Generic;
using System.Text;

namespace BoosterPumpLibrary.Commands
{
    public abstract class SimpleCommandBase : CommandBase
    {
        public override IEnumerable<byte> I2C_Data() => new byte[0];
    }
}
