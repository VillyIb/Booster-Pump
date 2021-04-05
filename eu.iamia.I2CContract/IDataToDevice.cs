using System.Collections.ObjectModel;

namespace eu.iamia.NCD.Serial.Contract
{
    /// <summary>
    /// Raw I2C Command.
    /// </summary>
    public interface IDataToDevice
    {
        /// <summary>
        /// I2C Command.
        /// </summary>
        ReadOnlyCollection<byte> Payload { get; }
    }
}
