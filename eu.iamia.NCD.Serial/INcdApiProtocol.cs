using System.Collections.Immutable;

namespace eu.iamia.NCD.Serial
{
    public interface INcdApiProtocol
    {
        byte Header { get; }
        byte ByteCount { get; }
        IImmutableList<byte> Payload { get; }
        byte Checksum { get; }
    }
}