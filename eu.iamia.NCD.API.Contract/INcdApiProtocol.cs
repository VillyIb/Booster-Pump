using System.Collections.Generic;
using System.Collections.Immutable;

// ReSharper disable InvalidXmlDocComment

namespace eu.iamia.NCD.API.Contract
{
    /// <summary>
    /// NCD API Format: {Header, ByteCount, Payload, Checksum}
    /// </summary>
    /// <see cref="https://ncd.io/serial-to-i2c-conversion/"/>
    public interface INcdApiProtocol
    {
        /// <summary>
        /// NCP API Header Byte (0xAA).
        /// </summary>
        byte Header { get; }

        /// <summary>
        /// NCD API Byte Count.
        /// </summary>
        byte ByteCount { get; }

        IImmutableList<byte> Payload { get; }

        ulong Value { get; }

        /// <summary>
        /// NCD API Checksum
        /// </summary>
        byte Checksum { get; }

        /// <summary>
        /// Returns an enumerator that iterates through Header, ByteCount, Payload and Checksum.
        /// </summary>
        /// <returns></returns>
        IEnumerable<byte> GetApiEncodedData();

        /// <summary>
        /// Returns true when Checksum can be verified.
        /// </summary>
        bool IsValid { get; }

        /// <summary>
        /// Returns true for one of the Error Codes  90 (0x5A), 91 (0X5A), 92 (0X5C) 
        /// </summary>
        bool IsError { get; }
    }
}