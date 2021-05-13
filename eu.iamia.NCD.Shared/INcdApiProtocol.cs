using System.Collections.Generic;
using System.Collections.Immutable;

namespace eu.iamia.NCD.Shared
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
    }
}