using System.Collections.Immutable;

namespace eu.iamia.NCD.DeviceCommunication.Contract
{
    /// <summary>
    /// 
    /// </summary>
    /// <see cref="https://ncd.io/serial-to-i2c-conversion/"/>
    public interface INcdApiProtocol
    {
        public byte Header { get; }

        /// <summary>
        /// Number of bytes in Payload.
        /// </summary>
        public byte ByteCount { get; }

        public IImmutableList<byte> Payload { get; }

        public byte Checksum { get; }

        /// <summary>
        /// Returns true if Checksum is correct.
        /// </summary>
        public bool IsValid { get; }
    }
}