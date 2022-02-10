// ReSharper disable UnusedMemberInSuper.Global

namespace eu.iamia.SerialPortSetting.Contract
{
    public interface IMeasurementSettings
    {
        int RoundTripTime { get; set; }
        float ManifoldPressureCorrection { get; set; }
        float FlowNorthWestCorrection { get; set; }
        float FlowSouthEastCorrection { get; set; }
        float SystemPressureCorrection { get; set; }
    }
}