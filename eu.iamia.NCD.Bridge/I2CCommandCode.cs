namespace eu.iamia.NCD.Bridge
{
    public enum I2CCommandCode : byte
    {
        Undefined = 0,
        DeviceWrite = 0xBE,
        DeviceRead = 0xBF,
        DeviceWriteRead = 0xC0,
        DeviceBusScan = 0xC1,
        DeviceConverterCommand = 0xFE
    }
}