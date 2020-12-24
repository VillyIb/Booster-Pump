using System;
using System.Threading;
using System.Threading.Tasks;

namespace BoosterPumpLibrary.Logger
{
    public interface IBufferedLogWriter
    {
        void Add(string row, DateTime timestamp);

        void Add(BufferLine payload);

        void AggregateFlush(DateTime window);

        void AggregateFlushUnconditionally();

        void AggregateExecute();

        Task AggregateExecuteAsync(CancellationToken cancellationToken);

        bool IsNextMinute();
    }
}
