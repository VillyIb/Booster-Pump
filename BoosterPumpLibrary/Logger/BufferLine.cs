using System;

namespace BoosterPumpLibrary.Logger
{
    public partial class BufferedLogWriter
    {
        private class BufferLine
        {
            public DateTime Timestamp { get; set; }

            public string LogText { get; set; }

            public BufferLine(string logText, DateTime timestamp)
            {
                Timestamp = timestamp;
                LogText = logText;
            }
        }
    }
}
