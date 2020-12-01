namespace BoosterPumpConfiguration
{
    /// <summary>
    /// Only read at startup
    /// </summary>
    public class SerialPortSettings
    {
        public static string Name => "SerialPort";

        public string PortName { get; set; }

        public int BaudRate { get; set; }

        public int Timeout { get; set; }
    }

    /// <summary>
    /// Only read at startup
    /// </summary>
    public class DatabaseSettings
    {
        public static string Name => "Database";

        public string SubDirectory { get; set; }

        public string FilePrefix { get; set; }

        public int FlushInterval { get; set; }
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

        public bool Enabled { get; set; }

        public float MinSpeedPct { get; set; }

        public float EastGradient { get; set; }

        public float EastIntercept { get; set; }

        public float WestGreadient { get; set; }

        public float SestIntercept { get; set; }

        public float CommonGradient { get; set; }

        public float CommonIntercept { get; set; }
    }

    /// <summary>
    /// Only read at startup
    /// </summary>
    public class AlarmSettings
    {
        public static string Name => "Alarm";

        public string MinSystemPressure { get; set; }
    }
}
