// ReSharper disable UnusedMemberInSuper.Global

using System.Collections.ObjectModel;

namespace eu.iamia.NCDAPI.Contract
{
    /// <summary>
    /// Raw I2C Command Response.
    /// </summary>
    public interface IDataFromDevice
    {
        /// <summary>
        /// I2C Command Response.
        /// </summary>
        ReadOnlyCollection<byte> Payload { get; }

        /// <summary>
        /// Indicate a valid Checksum.
        /// </summary>
        bool IsValid { get; }
    }
}
