namespace eu.iamia.i2c.communication.contract;

public interface IBitSetting
{
    ulong Mask { get; }

    /// <summary>
    /// BitSetting start position value: 0..N where N = 
    /// value is shifted this number of bits.
    /// </summary>
    ushort Offset { get; }

    /// <summary>
    /// Number of bits in setting , value: 1..8 (1..16/1..24/1....)
    /// </summary>
    ushort Size { get; }

    string Description { get; }

    ulong Value { get; set; }

    string MaskAsBinary();

    string ToString();
}