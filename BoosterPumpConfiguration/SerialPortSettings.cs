namespace BoosterPumpConfiguration
{
    using eu.iamia.SerialPortSetting.Contract;

    /// <summary>
    /// Only read at startup
    /// </summary>
    public class SerialPortSettings : ISerialPortSettings
    {
        public SerialPortSettings()
        {

        }

        public static string Name => "SerialPort";

        // ReSharper disable once UnusedMember.Global
        public string PortName { get; set; }

        public int BaudRate { get; set; }

        public int Timeout { get; set; }
    }
}