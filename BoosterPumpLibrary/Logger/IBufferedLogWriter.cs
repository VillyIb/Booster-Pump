using System;
using System.Threading.Tasks;
// ReSharper disable UnusedMemberInSuper.Global

namespace BoosterPumpLibrary.Logger
{
    public interface IBufferedLogWriter
    {
        void Add(BufferLine payload);


        void AggregateFlush(DateTime window);

        void AggregateFlushUnconditionally();

        Task AggregateFlushAsync(DateTime window);

        Task AggregateFlushUnconditionalAsync();

        Task WaitUntilSecond02InNextMinuteAsync();

        bool IsNextMinute();
    }
}
