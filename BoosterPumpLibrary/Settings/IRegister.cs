using System.Collections.Generic;

namespace BoosterPumpLibrary.Settings;

public interface IRegister
{
    /// <summary>
    /// Information.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Indicate the Register is read from an I2C Device
    /// </summary>
    bool IsInput { get; }

    /// <summary>
    /// Indicate input data is valid.
    /// </summary>
    bool IsInputDirty { get; set; }

    /// <summary>
    /// Indicate this Register is written to an I2C device.
    /// </summary>
    bool IsOutput { get; }

    /// <summary>
    /// Indicate output data is valid.
    /// </summary>
    bool IsOutputDirty { get; set; }

    /// <summary>
    /// Number of bytes this Register is managing {1..8}
    /// </summary>
    ushort Size { get; }

    byte RegisterAddress { get; }
    ulong Value { get; set; }

    /// <summary>
    /// Defines a subview <em>numberOfBits</em> bits long shifted <em>offsetInBits</em> bits with <em>description</em> as name.
    /// </summary>
    /// <param name="numberOfBits"></param>
    /// <param name="offsetInBits"></param>
    /// <param name="description"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    IBitSetting GetOrCreateSubRegister(ushort numberOfBits, ushort offsetInBits, string description = "");

    string ToString();

    /// <summary>
    /// Returns Value as byte list of length ByteCount.
    /// Clears IsOutputDirty
    /// </summary>
    /// <returns></returns>
    IEnumerable<byte> GetByteValuesToWriteToDevice();
}