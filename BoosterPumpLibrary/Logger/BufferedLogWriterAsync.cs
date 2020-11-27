using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace BoosterPumpLibrary.Logger
{
    public class BufferedLogWriterAsync : IBufferedLogWriter, IComponent
    {
        private readonly IOutputFileHandler FileHandler;

        private readonly ConcurrentQueue<BufferLine> Queue;

        private readonly int RoundTripDuration;

        public BufferedLogWriterAsync(IOutputFileHandler fileHandler)
        {
            FileHandler = fileHandler;
            Queue = new ConcurrentQueue<BufferLine>();

            RoundTripDuration = 60; // One minute
        }

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
        public async Task Flush(DateTime window, bool flushAll = false)
        {
            await Console.Error.WriteLineAsync("\r\n\n\n\n\n\n\n\n");

            var threshold = RoundToMinute(window);

            await Console.Error.WriteLineAsync($"Probing Flush: {threshold.ToLocalTime()}, now: {DateTime.Now}");

            while (!Queue.IsEmpty && Queue.TryPeek(out BufferLine current))
            {
                if (!flushAll && threshold <= RoundToMinute(current.Timestamp)) { break; }

                try
                {
                    await FileHandler.WriteLine(current.Timestamp, current.LogText);
                    Queue.TryDequeue(out current);

                    await Console.Error.WriteLineAsync(current.Timestamp.ToString("O"));
                }
                catch (Exception ex)
                {
                    Queue.Enqueue(new BufferLine(ex.Message, DateTime.UtcNow));
                    await Task.Delay(1000);
                }
            }
            await FileHandler.Close();
        }

        /// <summary>
        /// Flushes all waiting messages with the specified (minute) window, optionally flush all.
        /// </summary>
        /// <param name="window"></param>
        /// <param name="flushAll"></param>
        /// <returns></returns>
        public async Task AggregateFlush(DateTime window, bool flushAll = false)
        {
            await Console.Error.WriteLineAsync("\r\n\n\n\n\n\n\n\n");

            var threshold = RoundToMinute(window);

            await Console.Error.WriteLineAsync($"Probing AggregateFlush: {threshold.ToLocalTime()}, now: {DateTime.Now}");

            var aggregateValue = "";

            while (!Queue.IsEmpty && Queue.TryPeek(out BufferLine current))
            {
                if (!flushAll && threshold <= RoundToMinute(current.Timestamp)) { break; }

                try
                {
                    aggregateValue = $"{aggregateValue}, {current.LogText}";
                    Queue.TryDequeue(out current);

                    await Console.Error.WriteLineAsync(current.Timestamp.ToString("O"));
                }
                catch (Exception ex)
                {
                    Queue.Enqueue(new BufferLine(ex.Message, DateTime.UtcNow));
                    await Task.Delay(1000);
                }
            }
            if (!String.IsNullOrEmpty(aggregateValue))
            {
                await FileHandler.WriteLine(threshold, aggregateValue);
            }
            await FileHandler.Close();
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            do
            {
                var window = DateTime.UtcNow;
                await Flush(window);

                var delay = RoundToMinute(DateTime.UtcNow).AddSeconds(RoundTripDuration + 2).Subtract(DateTime.UtcNow);
                try
                {
                    await Task.Delay(Math.Max((int)delay.TotalMilliseconds, 10000), cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    await Flush(window, true);
                    break;
                }
            }
            while (true);
        }

        public async Task AggregateExecuteAsync(CancellationToken cancellationToken)
        {
            do
            {
                var window = DateTime.UtcNow;
                await AggregateFlush(window);

                var delay = RoundToMinute(DateTime.UtcNow).AddSeconds(RoundTripDuration + 2).Subtract(DateTime.UtcNow);
                try
                {
                    await Task.Delay(Math.Max((int)delay.TotalMilliseconds, 10000), cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    await AggregateFlush(RoundToMinute(window), true);
                    break;
                }
            }
            while (true);
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
