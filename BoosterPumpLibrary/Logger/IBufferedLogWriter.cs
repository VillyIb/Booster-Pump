using System;

namespace BoosterPumpLibrary.Logger
{
    public interface IBufferedLogWriter
    {
        void Add(string row, DateTime timestamp);
    }
}
