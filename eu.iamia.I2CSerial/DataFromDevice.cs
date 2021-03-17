using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using eu.iamia.NCDAPI.Contract;

namespace eu.iamia.NCDAPI
{
    /// <summary>
    /// Data returned from device.
    /// </summary>
    public class DataFromDevice : IDataFromDevice
    {
        public byte Header { get;  }

        public byte ByteCount { get;  }

        public ReadOnlyCollection<byte> Payload { get; }

        public bool IsValid => CheckConsistency;

        public byte Checksum { get;  }

        public DataFromDevice(byte header, byte byteCount, ReadOnlyCollection<byte> payload, byte checksum)
        {
            Header = header;
            ByteCount = byteCount;
            Payload = payload;
            Checksum = checksum;
        }

        public DataFromDevice(IEnumerable<byte> payload)
        {
            Header = 0xAA;
            Payload = new ReadOnlyCollection<byte>(payload.ToList());
            ByteCount = (byte)Payload.Count;
            Checksum = CalculatedChecksum;
        }

        public byte CalculatedChecksum
        {
            get
            {
                if (ByteCount != Payload.Count)
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
                if (ByteCount != Payload.Count) { return false; }
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
