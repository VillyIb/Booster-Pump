using System.Collections.Generic;
using System.Linq;
using System.Text;
using eu.iamia.NCD.DeviceCommunication.Contract;
using System.Collections.Immutable;

namespace eu.iamia.NCD.Serial
{
    /// <summary>
    /// Data returned from device.
    /// </summary>
    public class DataFromDevice : IDataFromDevice
    {
        public byte Header { get;  }

        public byte ByteCount { get;  }

        private readonly ImmutableList<byte> PayloadField;

        public IImmutableList<byte> Payload => PayloadField;

        public bool IsValid => CheckConsistency;

        public byte Checksum { get;  }

        public DataFromDevice(byte header, byte byteCount, IEnumerable<byte> payload, byte checksum)
        {
            Header = header;
            ByteCount = byteCount;
            PayloadField = ImmutableList<byte>.Empty.AddRange(payload);
            Checksum = checksum;
        }

        public DataFromDevice(IEnumerable<byte> payload)
        {
            Header = 0xAA;
            PayloadField = ImmutableList<byte>.Empty.AddRange(payload);
            ByteCount = (byte)PayloadField.Count;
            Checksum = CalculatedChecksum;
        }

        public byte CalculatedChecksum
        {
            get
            {
                if (ByteCount != PayloadField.Count)
                {
                    return byte.MinValue;
                }
                var checksum = Header + ByteCount;
                checksum = PayloadField.Aggregate(checksum, (current1, current) => current1 + current) & 0xff;

                return (byte)checksum;
            }
        }

        private bool CheckConsistency
        {
            get
            {
                if (null == PayloadField) { return false; }
                if (ByteCount != PayloadField.Count) { return false; }
                return CalculatedChecksum == Checksum;
            }
        }

        public IEnumerable<byte> ApiEncodedData()
        {
            yield return Header;
            yield return ByteCount;
            if (null != PayloadField)
            {
                foreach (var current in PayloadField)
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
