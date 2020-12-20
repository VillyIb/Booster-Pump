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

        public bool IsValid => CheckConsistency;

        public byte Checksum { get; set; }

        public byte CalculatedChecksum
        {
            get
            {
                if (ByteCount != Payload.Length)
                {
                    return byte.MinValue;
                }
                var checksum = Header + ByteCount;
                checksum = Payload.Aggregate(checksum, (current1, current) => current1 + current) & 0xff;

                return (byte)checksum;
            }
        }

        private bool CheckConsistency
        {
            get
            {
                if (null == Payload) { return false; }
                if (ByteCount != Payload.Length) { return false; }
                return CalculatedChecksum == Checksum;
            }
        }

        public IEnumerable<byte> ApiEncodedData()
        {
            yield return Header;
            yield return ByteCount;
            if (null != Payload)
            {
                foreach (var current in Payload)
                {
                    yield return current;
                }
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
