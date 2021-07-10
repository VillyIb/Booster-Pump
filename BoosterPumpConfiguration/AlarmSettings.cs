namespace BoosterPumpConfiguration
{
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