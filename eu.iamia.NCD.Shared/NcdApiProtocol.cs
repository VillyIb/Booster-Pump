#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using EnsureThat;
using eu.iamia.NCD.API.Contract;

namespace eu.iamia.NCD.Shared
{
    public sealed class NcdApiProtocol : INcdApiProtocol, IEquatable<INcdApiProtocol>
    {
        public byte Header { get; }

        public byte ByteCount { get; }

        private readonly IList<byte> PayloadField;

        public IImmutableList<byte> Payload => ImmutableList<byte>.Empty.AddRange(PayloadField);

        public byte Checksum { get; }

        private byte? CalculatedChecksumField;

        internal byte CalculatedChecksum
        {
            get
            {
                if (CalculatedChecksumField is not null)
                {
                    return CalculatedChecksumField.Value;
                }
                var checksum = Header + ByteCount;
                checksum = PayloadField.Aggregate(checksum, (current1, current) => current1 + current) & 0xff;
                CalculatedChecksumField = (byte)checksum;
                return CalculatedChecksumField.Value;
            }
        }

        private bool CheckConsistency
        {
            get
            {
                if (ByteCount != PayloadField.Count)
                {
                    return false;
                }
                return CalculatedChecksum == Checksum;
            }
        }

        public bool IsValid => CheckConsistency;

        public bool IsError
        {
            get
            {
                var result = false;

                result |= Equals(NcdApiProtocol.NoResponse);
                result |= Equals(NcdApiProtocol.InvalidAddress);
                result |= Equals(NcdApiProtocol.Timeout);

                return result;
            }
        }

        private ulong? ValueField;

        public ulong Value
        {
            get
            {
                if (ValueField is not null)
                {
                    return ValueField.Value;
                }

                ValueField = 0UL;

                foreach (var current in Payload)
                {
                    ValueField = (ValueField << 8) + current;
                }

                return ValueField.Value;
            }
        }

        public NcdApiProtocol(byte header, byte byteCount, IEnumerable<byte> payload, byte checksum)
        {
            EnsureArg.IsNotNull(payload, nameof(payload));
            PayloadField = payload.ToList();
            Ensure.That(PayloadField, nameof(payload)).SizeIs(Math.Min(255, PayloadField.Count));

            Header = header;
            ByteCount = byteCount;
            Checksum = checksum;

            GetHashCodeField = Header.GetHashCode() ^ ByteCount.GetHashCode() ^ Checksum.GetHashCode() ^ Value.GetHashCode();
        }

        public NcdApiProtocol(IEnumerable<byte> payload) : this(0xAA, 0, payload, 0)
        {
            ByteCount = (byte)PayloadField.Count;
            Checksum = CalculatedChecksum;

            GetHashCodeField = Header.GetHashCode() ^ ByteCount.GetHashCode() ^ Checksum.GetHashCode() ^ Value.GetHashCode();
        }

        public IEnumerable<byte> GetApiEncodedData()
        {
            yield return Header;
            yield return ByteCount;
            foreach (var current in Payload)
            {
                yield return current;
            }
            yield return Checksum;
        }

        // ReSharper disable once UnusedMember.Global
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

        public bool Equals(INcdApiProtocol? other)
        {
            if (other is null)
            {
                return false;
            }

            if (GetHashCode() != other.GetHashCode())
            {
                return false;
            }

            var result = true;
            result &= Value == other.Value;
            result &= ByteCount == other.ByteCount;
            result &= Checksum == other.Checksum;
            result &= Header == other.Header;

            return result;
        }

        private readonly int GetHashCodeField;

        public override int GetHashCode()
        {
            return GetHashCodeField;
        }

        public override bool Equals(object? obj)
        {
            return Equals((INcdApiProtocol?)obj);
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

        public static NcdApiProtocol WriteSuccess => new NcdApiProtocol(new byte[] { 0x55 });

        public static NcdApiProtocol NoResponse => new NcdApiProtocol(new byte[] { 0xBC, 0x5A, 0xA5, 0x43 });

        public static NcdApiProtocol Timeout => new NcdApiProtocol(new byte[] { 0xBC, 0x5B, 0xA4, 0x43 });

        public static NcdApiProtocol InvalidAddress => new NcdApiProtocol(new byte[] { 0xBC, 0x5C, 0xA3, 0x43 });

    }
}