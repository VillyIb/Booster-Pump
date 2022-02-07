namespace BoosterPumpConfiguration;

public interface IMeasurementSettings
{
    int RoundTripTime { get; set; }
    float ManifoldPressureCorrection { get; set; }
    float FlowNorthWestCorrection { get; set; }
    float FlowSouthEastCorrection { get; set; }
    float SystemPressureCorrection { get; set; }
}