using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace BoosterPumpLibrary.Logger
{
    public class BufferedLogWriterAsync : IBufferedLogWriter, IComponent
    {
        private readonly IOutputFileHandler AggregateFile;

        private readonly ConcurrentQueue<BufferLine> Queue;

        public BufferedLogWriterAsync(IOutputFileHandler aggregateFile)
        {
            AggregateFile = aggregateFile;
            Queue = new ConcurrentQueue<BufferLine>();
        }

        /// <summary>
        /// Returns the starttime for the current minute 'yyyy-MM-dd HH:mm:00.0000000'
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime RoundToMinute(DateTime value)
        {
            return new DateTime(value.Ticks - value.Ticks % TimeSpan.TicksPerMinute, value.Kind);
        }
               
        /// <summary>
        /// Flushes all waiting messages with the specified (minute) window, optionally flush all.
        /// </summary>
        /// <param name="window"></param>
        /// <param name="flushAll"></param>
        /// <returns></returns>
        public async Task AggregateFlush(DateTime window, bool flushAll = false)
        {
            var threshold = RoundToMinute(window);

            await Console.Error.WriteLineAsync($"\r\nProbing AggregateFlush: {threshold.ToLocalTime()}, now: {DateTime.Now}");

            var aggregateValue = "";
            var aggregateCount = 0;

            while (!Queue.IsEmpty && Queue.TryPeek(out BufferLine current))
            {
                if (!flushAll && threshold <= RoundToMinute(current.Timestamp)) { break; }

                try
                {
                    aggregateValue = $"{aggregateValue}, {current.LogText}"; // TODO modify aggregation operation
                    // TODO optionally write to full-log file.
                    aggregateCount++;
                    Queue.TryDequeue(out current);

                    await Console.Error.WriteLineAsync($"    Dequeued: {current.Timestamp.ToLocalTime().ToString("O")}");
                }
                catch (Exception ex)
                {
                    Queue.Enqueue(new BufferLine(ex.Message, DateTime.UtcNow));
                    await Task.Delay(1000);
                }
            }
            if (!String.IsNullOrEmpty(aggregateValue))
            {
                await AggregateFile.WriteLine(threshold, aggregateValue); // TODO modify aggregation operation divide by aggregateCount.
            }
            await AggregateFile.Close();
        }
           
        /// <summary>
        /// Waits until NextMinute + 2 seconds in order to let other tasks finish before kicking in.
        /// </summary>
        /// <returns></returns>
        public async Task WaitUntilSecond02InNextMinute()
        {
            var now = DateTime.UtcNow;
            var thisMinute = RoundToMinute(now);
            var nextMinute02 = thisMinute.AddSeconds(62);
            var delay = nextMinute02.Subtract(now);
            await Task.Delay(delay);
        }

        public async Task AggregateExecuteAsync(CancellationToken cancellationToken)
        {
            do
            {
                await WaitUntilSecond02InNextMinute();
                await AggregateFlush(DateTime.UtcNow);
            }
            while (!cancellationToken.IsCancellationRequested);
        }

        public void Add(string row, DateTime timestampUtc)
        {
            Queue.Enqueue(new BufferLine(row, timestampUtc));
        }

        public ISite Site { get => throw new NotImplementedException(); set { } }

        public event EventHandler Disposed;

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
