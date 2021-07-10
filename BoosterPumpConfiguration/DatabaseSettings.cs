namespace BoosterPumpConfiguration
{
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
}