using System;
using System.Linq;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Globalization;
using System.Text;

namespace BoosterPumpLibrary.Logger
{
    public class BufferedLogWriterAsync : IBufferedLogWriter, IComponent
    {
        private static CultureInfo CultureInfo => CultureInfo.GetCultureInfo("da-DK"); //  da-DK

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
        public async Task AggregateFlushAsync(DateTime window, bool flushAll = false)
        {
            var threshold = RoundToMinute(window);

            await Console.Error.WriteLineAsync($"\r\nProbing AggregateFlush: {threshold.ToLocalTime()}, now: {DateTime.Now}");

            var aggregateValue = "";
            var aggregateMeasures = new List<float> { 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f };
            var aggregateCount = 0;
            var consoleBuffer = new StringBuilder();

            while (!Queue.IsEmpty && Queue.TryPeek(out BufferLine current))
            {
                if (!flushAll && threshold <= RoundToMinute(current.Timestamp)) { break; }

                try
                {
                    if (current is BufferLineMeasurement measurement)
                    {
                        aggregateCount++;
                        for (var index = 0; index < aggregateMeasures.Count; index++)
                        {
                            aggregateMeasures[index] += measurement.Values[index];

                            var seed = string.Format(CultureInfo, "{1:O}{0}{2:000000}{0}", AggregateFile.SeparatorCharacter, current.Timestamp.ToLocalTime(), current.Timestamp.ToLocalTime().TimeOfDay.TotalMilliseconds / 100);
                            var line = aggregateMeasures.Aggregate(
                                seed,
                                (result, current) => result + (current).ToString("0000.0", CultureInfo) +
                                                     AggregateFile.SeparatorCharacter
                            );
                            await AggregateFile.WriteLineAsync(threshold, "S", line);
                        }
                    }
                    else
                    {
                        aggregateValue = $"{aggregateValue}, {current.LogText}";
                    }

                    // TODO optionally write to full-log file.
                    Queue.TryDequeue(out current);

                    //await Console.Error.WriteLineAsync($"    Dequeued: {current.Timestamp.ToLocalTime().ToString("O")}");
                    // ReSharper disable once PossibleNullReferenceException
                    consoleBuffer.Append($"    Dequeued: {current.Timestamp.ToLocalTime():O}\r\n");
                }
                catch (Exception ex)
                {
                    Queue.Enqueue(new BufferLine(ex.Message, DateTime.UtcNow));
                    await Task.Delay(1000);
                }
            }
            if (!String.IsNullOrEmpty(aggregateValue))
            {
                await AggregateFile.WriteLineAsync(threshold, "M", aggregateValue);
            }

            if (aggregateCount > 0)
            {
                var seed = string.Format(CultureInfo, "{1:O}{0}{2:000000}{0}", AggregateFile.SeparatorCharacter, threshold.ToLocalTime(), threshold.ToLocalTime().TimeOfDay.TotalMilliseconds / 100);

                var line = aggregateMeasures.Aggregate(
                    seed,
                    (result, current) => result + (current / aggregateCount).ToString("0000.0", CultureInfo) + AggregateFile.SeparatorCharacter
                );
                await AggregateFile.WriteLineAsync(threshold, "M", line);
            }

            await AggregateFile.Close();
            Console.WriteLine(consoleBuffer.ToString());
        }

        /// <summary>
        /// Waits until NextMinute + 2 seconds in order to let other tasks finish before kicking in.
        /// </summary>
        /// <returns></returns>
        public async Task WaitUntilSecond02InNextMinuteAsync()
        {
            var now = DateTime.UtcNow;
            var thisMinute = RoundToMinute(now);
            var nextMinute02 = thisMinute.AddSeconds(62);
            var delay = nextMinute02.Subtract(now);
            await Task.Delay(delay);
        }

        public async Task AggregateExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                Console.WriteLine("\r\n+BufferedLogWriterAsync.ExecuteAsync");
                do
                {
                    await WaitUntilSecond02InNextMinuteAsync();
                    await AggregateFlushAsync(DateTime.UtcNow);
                }
                while (!cancellationToken.IsCancellationRequested);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
            }
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

        public void Add(BufferLine payload)
        {
            Queue.Enqueue(payload);
        }
    }
}
