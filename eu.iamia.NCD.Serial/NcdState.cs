namespace eu.iamia.NCD.Serial
{
    /// <summary>
    /// Values: {Undefined|ExpectHeader|ExpectLength|ExpectPayload|ExpectChecksum|Overflow}
    /// </summary>
    public enum NcdState
    {
        Undefined = 0,
        ExpectHeader = 1,
        ExpectLength = 2,
        ExpectPayload = 3,
        ExpectChecksum = 4,
        Overflow = 5
    }
}