namespace NCD_API_SerialConverter.Commands
{
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Data retured from device.
    /// </summary>
    public class NCD_API_Packet_Read_Data
    {
        public byte Header { get; set; }

        public byte ByteCount { get; set; }

        public byte[] Payload { get; set; }

        public byte Checksum { get; set; }

        public bool VerifyChecksum
        {
            get
            {
                var checksum = Header + ByteCount;
                foreach (var current in Payload)
                {
                    checksum += current;
                }
                return Checksum == (byte)(checksum & 0xff);
            }
        }

        public IEnumerable<byte> ApiEncodedData()
        {
            yield return Header;
            yield return ByteCount;
            foreach (var current in Payload)
            {
                yield return current;
            }
            yield return Checksum;
        }

        public override string ToString()
        {
            var result = new StringBuilder();

            foreach (var current in ApiEncodedData())
            {
                result.AppendFormat($"{current:X2} ");
            }

            return result.ToString();
        }
    }
}
