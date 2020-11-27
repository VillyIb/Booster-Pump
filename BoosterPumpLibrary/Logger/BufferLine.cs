using System;

namespace BoosterPumpLibrary.Logger
{

    public class BufferLine
    {
        public DateTime Timestamp { get; }

        public string LogText { get; }

        public BufferLine(string logText, DateTime timestamp)
        {
            Timestamp = timestamp;
            LogText = logText;
        }
    }

}
