namespace eu.iamia.NCD.Serial
{
    /// <summary>
    /// Values: {Undefined|ExpectHeader|ExpectLength|ExpectPayload|ExpectChecksum|Overflow}
    /// </summary>
    internal enum NcdState
    {
        // ReSharper disable once UnusedMember.Global
        Undefined = 0,
        ExpectHeader = 1,
        ExpectLength = 2,
        ExpectPayload = 3,
        ExpectChecksum = 4,
        Overflow = 5
    }
}