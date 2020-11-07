using System.Collections.Generic;
using System.Text;

namespace BoosterPumpLibrary.Commands
{
    // see:https://ncd.io/serial-to-i2c-conversion/

    public abstract class CommandBase
    {
        public virtual byte Address { get; set; }

        public abstract IEnumerable<byte> I2C_Data();

        public string I2C_DataAsHex
        {
            get
            {
                var result = new StringBuilder();

                foreach (var current in I2C_Data())
                {
                    result.AppendFormat($"{current:X2} ");
                }

                return result.ToString();
            }
        }
    }
}
