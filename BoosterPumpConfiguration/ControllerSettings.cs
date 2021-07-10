//using eu.iamia.ReliableSerialPort;
//using Microsoft.Extensions.Configuration;

// ReSharper disable UnusedMember.Global

namespace BoosterPumpConfiguration
{
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
}
