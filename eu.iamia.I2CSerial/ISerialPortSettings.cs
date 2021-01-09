namespace eu.iamia.I2CSerial
{
    public interface ISerialPortSettings
    {
        string PortName { get; set; }

        int BaudRate { get; set; }

        int Timeout { get; set; }
    }
}