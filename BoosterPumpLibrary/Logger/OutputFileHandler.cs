using BoosterPumpConfiguration;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;

namespace BoosterPumpLibrary.Logger
{

    public interface IOutputFileHandler
    {
        Task WriteLine(DateTime timestamp, string line);

        Task Close();
    }

    [ExcludeFromCodeCoverage]
    public class OutputFileHandler : IOutputFileHandler, IDisposable
    {
        private readonly string LogfilePrefix;

        public OutputFileHandler(IOptions<DatabaseSettings> settings)
        {
            LogfilePrefix = settings.Value.FilePrefix;
        }

        private string CurrentFilename { get; set; }

        private StreamWriter Sw { get; set; }

        private string GetFilename(DateTime timestamp)
        {
            var daylightSaving = timestamp.IsDaylightSavingTime() ? "S" : "N";
            var filename = $"_{timestamp.Day:00}_{timestamp.Hour:00}{daylightSaving}.txt";
            return filename;
        }

        private async Task OpenFile(string filename)
        {
            CurrentFilename = filename;
            var file = new FileInfo($"{LogfilePrefix}{filename}");
            await using var fs = file.Open(FileMode.OpenOrCreate);
            Console.WriteLine($"\r\nWriting to logfile {file.Name}");
            Sw = new StreamWriter(fs);

            fs.Position = fs.Seek(0, SeekOrigin.End);
            if (fs.Position == 0L)
            {
                await Sw.WriteLineAsync("Timestamp\tSecond of day" +
                                        "\tPressure Manifold\tFlow NorthWest\tFlow SouthEast\tSystem Pressure" +
                                        "\tTBarometer 1\tBarometer 2\tTemperature1\tTemperature2"
                );
            }
        }

        /// <summary>
        /// Writes line to file.
        /// </summary>
        /// <param name="timestamp"></param>
        /// <param name="line"></param>
        /// <exception cref="">If a file could not be opened for write access</exception>
        public async Task WriteLine(DateTime timestamp, string line)
        {
            var filename = GetFilename(timestamp);
            if (!filename.Equals(CurrentFilename))
            {
                await Close();
                await OpenFile(filename);
            }
            await Sw.WriteLineAsync(line);
            await Sw.FlushAsync();
        }

        public async Task Close()
        {
            if (Sw != null)
            {
                try
                {
                    await Sw.FlushAsync();
                    Sw.Close();
                }
                catch
                { }
            }

            Sw = null;
            CurrentFilename = null;
        }

        protected virtual async Task Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    // release managed resources
                    await Close();
                }
                // release unmanaged resources

            }
            catch (Exception)
            {
                // No action
            }
            finally
            {
                Sw = null;
                CurrentFilename = null;
            }
        }

        public void Dispose()
        {
            Dispose(true).Wait();
            GC.SuppressFinalize(this);

        }
    }
}
