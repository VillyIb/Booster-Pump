// ReSharper disable UnusedMemberInSuper.Global

using System.Collections.Immutable;

namespace eu.iamia.NCD.API.Contract
{
    /// <summary>
    /// Raw I2C Command Response.
    /// </summary>
    public interface IDataFromDevice
    {
        /// <summary>
        /// I2C Command Response.
        /// </summary>
        IImmutableList<byte> Payload { get; }

        /// <summary>
        /// Indicate a valid Checksum.
        /// </summary>
        bool IsValid { get; }
    }
}
