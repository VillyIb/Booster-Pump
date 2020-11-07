using System.Text;

namespace BoosterPumpLibrary.Commands
{
    public class ReadData
    {
        public byte[] Payload { get; set; }

        public string I2C_DataAsHex
        {
            get
            {
                var result = new StringBuilder();

                foreach (var current in Payload)
                {
                    result.AppendFormat($"{current:X2} ");
                }

                return result.ToString();
            }
}
    }
}
