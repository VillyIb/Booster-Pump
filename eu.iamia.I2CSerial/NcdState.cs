namespace eu.iamia.I2CSerial
{
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