﻿using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using eu.iamia.NCD.API.Contract;

namespace eu.iamia.NCD.API
{
    public class NcdApiProtocolX 
    {
        public byte Header { get; }

        public byte ByteCount { get; }

        private readonly ImmutableList<byte> PayloadField;

        public IImmutableList<byte> Payload => PayloadField;

        public byte Checksum { get; }

        private byte CalculatedChecksum
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
                if (null == PayloadField)
                {
                    return false;
                }

                if (ByteCount != PayloadField.Count)
                {
                    return false;
                }

                return CalculatedChecksum == Checksum;
            }
        }

        public bool IsValid => CheckConsistency;


        public NcdApiProtocolX(byte header, byte byteCount, IEnumerable<byte> payload, byte checksum)
        {
            Header = header;
            ByteCount = byteCount;
            PayloadField = ImmutableList<byte>.Empty.AddRange(payload);
            Checksum = checksum;
        }

        public NcdApiProtocolX(IEnumerable<byte> payload)
        {
            Header = 0xAA;
            PayloadField = ImmutableList<byte>.Empty.AddRange(payload);
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

    /// <summary>
    /// Data returned from device.
    /// </summary>
    public class DataFromDevice : NcdApiProtocolX, IDataFromDevice
    {
        public DataFromDevice(byte header, byte byteCount, IEnumerable<byte> payload, byte checksum)
            : base(header, byteCount, payload, checksum)
        { }

        public DataFromDevice(IEnumerable<byte> payload) : base(payload)
        { }

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

