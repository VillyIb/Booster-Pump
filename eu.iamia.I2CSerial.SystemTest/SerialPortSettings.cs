namespace eu.iamia.I2CSerial.SystemTest
{
    public class SerialPortSettings : ISerialPortSettings
    {
        public static string Name => "SerialPort";

        // ReSharper disable once UnusedMember.Global
        public string PortName { get; set; }

        public int BaudRate { get; set; }

        public int Timeout { get; set; }
    }
}
