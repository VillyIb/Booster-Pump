using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using EnsureThat;

namespace eu.iamia.NCD.Shared
{
    public class NcdApiProtocol : INcdApiProtocol
    {
        public byte Header { get; }

        public byte ByteCount { get; }

        private readonly IList<byte> PayloadField;

        public IImmutableList<byte> Payload => ImmutableList<byte>.Empty.AddRange(PayloadField);

        public byte Checksum { get; }

        private byte CalculatedChecksum
        {
            get
            {
                var checksum = Header + ByteCount;
                checksum = PayloadField.Aggregate(checksum, (current1, current) => current1 + current) & 0xff;
                return (byte)checksum;
            }
        }

        private bool CheckConsistency
        {
            get
            {
                if (PayloadField is null) { return false; }
                if (ByteCount != PayloadField.Count) { return false; }
                return CalculatedChecksum == Checksum;
            }
        }

        public bool IsValid => CheckConsistency;


        //private static Ens

        public NcdApiProtocol(byte header, byte byteCount, [NotNull] IEnumerable<byte> payload, byte checksum)
        {
            EnsureArg.IsNotNull(payload, nameof(payload));
            PayloadField = payload.ToList();
            Ensure.That(PayloadField, nameof(payload)).SizeIs(Math.Min(255, PayloadField.Count));

            Header = header;
            ByteCount = byteCount;
            Checksum = checksum;
        }

        public NcdApiProtocol([NotNull] IEnumerable<byte> payload) : this(0xAA, 0, payload, 0)
        {
            ByteCount = (byte)PayloadField.Count;
            Checksum = CalculatedChecksum;
        }

        public IEnumerable<byte> GetApiEncodedData()
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

        public string PayloadAsHex
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

        public override string ToString()
        {
            var result = new StringBuilder();

            foreach (var current in GetApiEncodedData())
            {
                result.AppendFormat($"{current:X2} ");
            }

            return result.ToString();
        }
    }
}