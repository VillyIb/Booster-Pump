using eu.iamia.I2CSerial;
using System;
using Microsoft.Extensions.Configuration;


namespace BoosterPumpConfiguration
{
    /// <summary>
    /// Only read at startup
    /// </summary>
    public class SerialPortSettings : ISerialPortSettings
    {
        public static string Name => "SerialPort";

        // ReSharper disable once UnusedMember.Global
        public string PortName { get; set; }

        public int BaudRate { get; set; }

        public int Timeout { get; set; }
    }

    public static class SerialPortSettingsExtension
    {
        private static bool TryGetValue<T>(IConfiguration section, string propertyName, out T value)
        {
            value = (T)section.GetValue(typeof(T), propertyName);
            return true;
        }

        public static SerialPortSettings Parse(this IConfiguration configuration)
        {
            IConfigurationSection section = configuration.GetSection(SerialPortSettings.Name);

            var serialPortSettings = new SerialPortSettings
            {
                BaudRate = TryGetValue(section, nameof(SerialPortSettings.BaudRate), out int baudRate) ? baudRate: 0,   
                PortName = TryGetValue(section, nameof (SerialPortSettings.PortName), out string portName) ? portName : "",
                Timeout = TryGetValue(section, nameof(SerialPortSettings.Timeout), out int timeOut) ? timeOut : 0
            };

            return serialPortSettings;
        }
    }

    /// <summary>
    /// Only read at startup
    /// </summary>
    public class DatabaseSettings
    {
        public static string Name => "Database";

        public string SubDirectory { get; set; }

        public string FilePrefix { get; set; }

        public string Headline { get; set; }

        public char SeparatorCharacter { get; set; }
    }

    /// <summary>
    /// Only read at startup
    /// </summary>
    public class MeasurementSettings
    {
        public static string Name => "Measurement";

        public int RoundTripTime { get; set; }

        public float ManifoldPressureCorrection { get; set; }

        public float FlowNorthWestCorrection { get; set; }

        public float FlowSouthEastCorrection { get; set; }

        public float SystemPressureCorrection { get; set; }
    }

    /// <summary>
    /// Reloaded before use.
    /// </summary>
    public class ControllerSettings
    {
        public static string Name => "Controller";

        // ReSharper disable once UnusedMember.Global
        public bool Enabled { get; set; }

        public float MinSpeedPct { get; set; }

        public float EastGradient { get; set; }

        public float WestGradient { get; set; }

        public float CommonGradient { get; set; }

        public float CommonIntercept { get; set; }

        public int AverageSize { get; set; }
    }

    /// <summary>
    /// Only read at startup
    /// </summary>
    public class AlarmSettings
    {
        public static string Name => "Alarm";

        // ReSharper disable once UnusedMember.Global
        public string MinSystemPressure { get; set; }
    }
}
