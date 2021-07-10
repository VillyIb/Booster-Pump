using Microsoft.Extensions.Configuration;

namespace BoosterPumpConfiguration
{
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
                BaudRate = TryGetValue(section, nameof(SerialPortSettings.BaudRate), out int baudRate) ? baudRate : 0,
                PortName = TryGetValue(section, nameof(SerialPortSettings.PortName), out string portName) ? portName : "",
                Timeout = TryGetValue(section, nameof(SerialPortSettings.Timeout), out int timeOut) ? timeOut : 0
            };

            return serialPortSettings;
        }
    }
}