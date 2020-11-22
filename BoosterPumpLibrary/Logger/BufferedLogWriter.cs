using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading;

namespace BoosterPumpLibrary.Logger
{
    [ExcludeFromCodeCoverage]
    public partial class BufferedLogWriter : IComponent
    {
        private readonly string LogDirectory;

        private static readonly TimeSpan OneMinute = new TimeSpan(0, 1, 0);

        // ReSharper disable once ConvertToAutoProperty
        private static TimeSpan BufferTime => OneMinute;

        private string Path => LogDirectory;

        private IList<BufferLine> Buffer { get; }

        private DateTime NextFlush { get; set; }
        private DateTime CurrentHourUtc { get; set; }

        public ISite Site
        {
            get => null;
            set { }
        }

        public BufferedLogWriter(string logDirectory)
        {
            LogDirectory = logDirectory;
            Buffer = new List<BufferLine>();
            NextFlush = DateTime.UtcNow.Add(BufferTime);
            CurrentHourUtc = DateTime.UtcNow;
        }

        public event EventHandler Disposed;

        private void FlushBuffer()
        {
            if (Buffer.Count == 0) { return; }

            var sentinel = 5;
            do
            {
                var timestamp = Buffer.Last().Timestamp;
                try
                {
                    var daylightSaving = timestamp.IsDaylightSavingTime() ? "S" : "N";

                    var filename = $"_{timestamp.Day:00}_{timestamp.Hour:00}{daylightSaving}.txt";
                    var file = new FileInfo($"{Path}{filename}");
                    using var fs = file.Open(FileMode.OpenOrCreate);
                    using var sw = new StreamWriter(fs);
                    fs.Position = fs.Seek(0, SeekOrigin.End);
                    if (fs.Position == 0L)
                    {
                        sw.WriteLine("Timestamp\tSecond of day" +
                                     "\tPressure Manifold\tFlow NorthWest\tFlow SouthEast\tSystem Pressure" +
                                     "\tTBarometer 1\tBarometer 2\tTemperature1\tTemperature2"
                        );
                    }

                    foreach (var current in Buffer)
                    {
                        sw.WriteLine(current.LogText);
                    }
                    sw.Flush();
                    Buffer.Clear();
                }
                catch (Exception ex)
                {
                    Buffer.Add(new BufferLine(ex.Message, DateTime.Now));
                    Thread.Sleep(1000);
                }
            }
            while (Buffer.Count > 0 && sentinel-- > 0);
        }

        private void ConditionallyFlushBuffer(DateTime timestamp)
        {
            var timestampHourUtc = timestamp.ToUniversalTime().Hour;

            if (DateTime.UtcNow < NextFlush
                && CurrentHourUtc.Hour == timestampHourUtc
            ) return;
            CurrentHourUtc = timestamp.ToUniversalTime();
            NextFlush = DateTime.UtcNow.Add(BufferTime);
            FlushBuffer();
        }

        public void Add(string row, DateTime timestamp)
        {
            ConditionallyFlushBuffer(timestamp);
            Buffer.Add(new BufferLine(row, timestamp));
        }

        protected virtual void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    // release managed resources
                }
                // release unmanaged resources
                FlushBuffer();
            }
            catch (Exception)
            {
                // No action.
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~BufferedLogWriter()
        {
            Dispose(false);
        }
    }
}
