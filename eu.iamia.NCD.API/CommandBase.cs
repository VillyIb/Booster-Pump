using System.Collections.Generic;
using System.Text;

namespace eu.iamia.NCD.API
{
    // see:https://ncd.io/serial-to-i2c-conversion/

    public abstract class CommandBase
    {
        // TODO NOT virtual - set to readonly
        public virtual byte DeviceAddress { get; set; }

        public abstract IEnumerable<byte> I2C_Data();

        protected CommandBase()
        { }

        protected CommandBase(byte deviceAddress)
        {
            DeviceAddress = deviceAddress;
        }

        public string I2CDataAsHex
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
