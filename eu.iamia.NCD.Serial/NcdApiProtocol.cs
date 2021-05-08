using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace eu.iamia.NCD.Serial
{
    public class NcdApiProtocol : INcdApiProtocol
    {
        public byte Header { get; }

        public byte ByteCount { get; }

        private readonly ImmutableList<byte> PayloadField;

        public IImmutableList<byte> Payload => PayloadField;

        public byte Checksum { get; }

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

        public bool IsValid => CheckConsistency;


        public NcdApiProtocol(byte header, byte byteCount, IEnumerable<byte> payload, byte checksum)
        {
            Header = header;
            ByteCount = byteCount;
            PayloadField = ImmutableList<byte>.Empty.AddRange(payload);
            Checksum = checksum;
        }

        public NcdApiProtocol(IEnumerable<byte> payload)
        {
            Header = 0xAA;
            PayloadField = ImmutableList<byte>.Empty.AddRange(payload);
            ByteCount = (byte)PayloadField.Count;
            Checksum = CalculatedChecksum;
        }

    }
}