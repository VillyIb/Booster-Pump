using System.Linq;

namespace NCD_API_SerialConverter.NcdApiProtocol
{
    using BoosterPumpLibrary.Contracts;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Data returned from device.
    /// </summary>
    public class DataFromDevice : IDataFromDevice
    {
        public byte Header { get; set; }

        public byte ByteCount { get; set; }

        public byte[] Payload { get; set; }

        public byte Checksum { get; set; }

        public bool CheckConsistency
        {
            get
            {
                if(ByteCount != Payload.Length) { return false; }

                var checksum = Header + ByteCount;
                checksum = Payload.Aggregate(checksum, (current1, current) => current1 + current) & 0xff;
                return Checksum == (byte)checksum;
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
