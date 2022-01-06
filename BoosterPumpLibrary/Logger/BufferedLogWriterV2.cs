using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eu.iamia.Util;

namespace BoosterPumpLibrary.Logger
{
    public class BufferedLogWriterV2 : IBufferedLogWriter, IComponent
    {
        private static CultureInfo CultureInfo => CultureInfo.GetCultureInfo("da-DK"); //  da-DK

        private readonly IOutputFileHandler AggregateFile;

        private readonly ConcurrentQueue<BufferLine> Queue;

        public BufferedLogWriterV2(IOutputFileHandler aggregateFile)
        {
            AggregateFile = aggregateFile;
            Queue = new();
        }

        /// <summary>
        /// Returns the start time for the current minute 'yyyy-MM-dd HH:mm:00.0000000'
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime RoundToMinute(DateTime value)
        {
            return new(value.Ticks - value.Ticks % TimeSpan.TicksPerMinute, value.Kind);
        }

        private DateTime NextMinute = DateTime.MinValue;

        /// <summary>
        /// Returns true when the current DateTime reaches or pass NextMinute.
        /// </summary>
        /// <returns></returns>
        public bool IsNextMinute()
        {
            if (NextMinute >= SystemDateTime.UtcNow) { return false; }

            var now = SystemDateTime.UtcNow;
            NextMinute = RoundToMinute(now).AddSeconds(62);
            return true;
        }

        protected void AggregateFlush(DateTime window, bool flushAll)
        {
            var threshold = RoundToMinute(window);

            Console.Error.WriteLine($"\r\nProbing AggregateFlush: {threshold.ToLocalTime()}, now: {DateTime.Now}");

            var aggregateValue = "";
            var aggregateMeasures = new List<float> { 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f };
            var aggregateCount = 0;
            var consoleBuffer = new StringBuilder();

            while (!Queue.IsEmpty && Queue.TryPeek(out var line))
            {
                if (!flushAll && threshold <= RoundToMinute(line.Timestamp)) { break; }

                Queue.TryDequeue(out line);
                try
                {
                    if (line is BufferLineMeasurement measurement)
                    {
                        aggregateCount++;
                        for (var index = 0; index < aggregateMeasures.Count; index++)
                        {
                            aggregateMeasures[index] += measurement.Values[index];
                        }

                        var seed = string.Format(CultureInfo, "{1:O}{0}{2:000000}{0}", AggregateFile.SeparatorCharacter, line.Timestamp.ToLocalTime(), line.Timestamp.ToLocalTime().TimeOfDay.TotalMilliseconds / 100);
                        var aggregate = measurement.Values.Aggregate(
                            seed,
                            (result, current) => result + current.ToString("0000.0", CultureInfo) +
                                                 AggregateFile.SeparatorCharacter
                        );
                        AggregateFile.WriteLine(threshold, "S", aggregate); // specific line

                    }
                    else
                    {
                        aggregateValue = aggregateValue.Any() ? $"{aggregateValue}, {line.LogText}" : line.LogText;
                    }

                    // TODO optionally write to full-log file.

                    // ReSharper disable once PossibleNullReferenceException
                    consoleBuffer.Append($"    Dequeued: {line.Timestamp.ToLocalTime():O}\r\n");
                }
                catch (Exception ex)
                {
                    Queue.Enqueue(new(ex.Message, SystemDateTime.UtcNow));
                }
            }
            if (!string.IsNullOrEmpty(aggregateValue))
            {
                AggregateFile.WriteLine(threshold, "T", aggregateValue);
            }

            if (aggregateCount > 0)
            {
                var seed = string.Format(CultureInfo, "{1:O}{0}{2:000000}{0}", AggregateFile.SeparatorCharacter, threshold.ToLocalTime(), threshold.ToLocalTime().TimeOfDay.TotalMilliseconds / 100);

                var line = aggregateMeasures.Aggregate(
                    seed,
                    (result, current) => result + (current / aggregateCount).ToString("0000.0", CultureInfo) + AggregateFile.SeparatorCharacter
                );
                AggregateFile.WriteLine(threshold, "M", line); // average
            }

            AggregateFile.Close();
            Console.WriteLine(consoleBuffer.ToString());
        }

        public void AggregateFlush(DateTime window)
        {
            AggregateFlush(window, false);
        }

        public void AggregateFlushUnconditionally()
        {
            AggregateFlush(DateTime.UtcNow, true);
        }

        /// <summary>
        /// Waits until NextMinute + 2 seconds in order to let other tasks finish before kicking in.
        /// </summary>
        /// <returns></returns>
        public async Task WaitUntilSecond02InNextMinuteAsync()
        {
            var now = SystemDateTime.UtcNow;
            var thisMinute = RoundToMinute(now);
            var nextMinute02 = thisMinute.AddSeconds(62);
            var delay = nextMinute02.Subtract(now);
            await Task.Delay(delay);
        }

        [ExcludeFromCodeCoverage]
        public ISite Site { get => throw new NotImplementedException(); set { } }

        public event EventHandler Disposed;

        [ExcludeFromCodeCoverage]
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        [ExcludeFromCodeCoverage]
        public void Add(BufferLine payload)
        {
            Queue.Enqueue(payload);
        }
    }
}
