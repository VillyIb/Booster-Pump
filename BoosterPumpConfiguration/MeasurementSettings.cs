namespace BoosterPumpConfiguration
{
    /// <summary>
    /// Only read at startup
    /// </summary>
    public class MeasurementSettings : IMeasurementSettings
    {
        public static string Name => "Measurement";

        public int RoundTripTime { get; set; }

        public float ManifoldPressureCorrection { get; set; }

        public float FlowNorthWestCorrection { get; set; }

        public float FlowSouthEastCorrection { get; set; }

        public float SystemPressureCorrection { get; set; }
    }
}