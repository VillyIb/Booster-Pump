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
        /// <summary>
        /// Character og separate columns in file.
        /// </summary>
        char SeparatorCharacter { get; }

        Task WriteLineAsync(DateTime timestamp, string line);

        Task Close();
    }

    [ExcludeFromCodeCoverage]
    public class OutputFileHandler : IOutputFileHandler, IDisposable
    {
        public DatabaseSettings Settings { get; }

        public OutputFileHandler(IOptions<DatabaseSettings> settings)
        {
            Settings = settings.Value;
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

            var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var logfilePrefix = Path.Combine(userProfile, Settings.SubDirectory, Settings.FilePrefix);
            var file = new FileInfo($"{logfilePrefix}{filename}");
            var fs = file.Open(FileMode.OpenOrCreate);
            Console.WriteLine($"\r\nWriting to logfile {file.Name}");
            Sw = new StreamWriter(fs) {AutoFlush = true};

            fs.Position = fs.Seek(0, SeekOrigin.End);
            if (fs.Position == 0L)
            {
                await Sw.WriteLineAsync(Settings.Headline.Replace(';', SeparatorCharacter));
            }
        }

        public char SeparatorCharacter => Settings.SeparatorCharacter; // TODO verify string '\t' translates to tab.

        /// <summary>
        /// Writes line to file.
        /// </summary>
        /// <param name="timestamp"></param>
        /// <param name="line"></param>
        /// <exception cref="">If a file could not be opened for write access</exception>
        public async Task WriteLineAsync(DateTime timestamp, string line)
        {
            var filename = GetFilename(timestamp);
            if (!filename.Equals(CurrentFilename))
            {
                await Close();
            }
            await OpenFile(filename);
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
                catch (Exception ex)
                {
                    await Console.Error.WriteLineAsync(ex.ToString());
                }
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
