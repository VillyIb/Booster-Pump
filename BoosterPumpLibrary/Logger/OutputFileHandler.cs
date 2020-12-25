using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using BoosterPumpConfiguration;

namespace BoosterPumpLibrary.Logger
{

    public interface IOutputFileHandler
    {
        /// <summary>
        /// Character og separate columns in file.
        /// </summary>
        char SeparatorCharacter { get; }

        void WriteLine(DateTime timestamp, string suffix, string line);

        Task WriteLineAsync(DateTime timestamp, string suffix, string line);

        void Close();

        Task CloseAsync();
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

        private static string GetFilename(DateTime timestamp, string suffix)
        {
            var daylightSaving = timestamp.IsDaylightSavingTime() ? "S" : "N";
            var filename = $"_{suffix}_{timestamp.Day:00}_{timestamp.Hour:00}{daylightSaving}.txt";
            return filename;
        }

        private void OpenFile(string filename)
        {
            CurrentFilename = filename;

            var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var logfilePrefix = Path.Combine(userProfile, Settings.SubDirectory, Settings.FilePrefix);
            var file = new FileInfo($"{logfilePrefix}{filename}");
            var fs = file.Open(FileMode.OpenOrCreate);
            Console.WriteLine($"\r\nWriting to logfile {file.Name}");
            Sw = new StreamWriter(fs) { AutoFlush = true };

            fs.Position = fs.Seek(0, SeekOrigin.End);
            if (fs.Position == 0L)
            {
                Sw.WriteLine(Settings.Headline.Replace(';', SeparatorCharacter));
            }
        }


        private async Task OpenFileAsync(string filename)
        {
            CurrentFilename = filename;

            var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var logfilePrefix = Path.Combine(userProfile, Settings.SubDirectory, Settings.FilePrefix);
            var file = new FileInfo($"{logfilePrefix}{filename}");
            var fs = file.Open(FileMode.OpenOrCreate);
            Console.WriteLine($"\r\nWriting to logfile {file.Name}");
            Sw = new StreamWriter(fs) { AutoFlush = true };

            fs.Position = fs.Seek(0, SeekOrigin.End);
            if (fs.Position == 0L)
            {
                await Sw.WriteLineAsync(Settings.Headline.Replace(';', SeparatorCharacter));
            }
        }

        public char SeparatorCharacter => Settings.SeparatorCharacter; // TODO verify string '\t' translates to tab.

        public void WriteLine(DateTime timestamp, string suffix, string line)
        {
            var filename = GetFilename(timestamp, suffix);
            if (!filename.Equals(CurrentFilename))
            {
                Close();
                OpenFile(filename);
            }
            Sw.WriteLine(line);
            Sw.Flush();
        }

        // ReSharper disable once InvalidXmlDocComment

        /// <summary>
        /// Writes line to file.
        /// </summary>
        /// <param name="timestamp"></param>
        /// <param name="suffix"></param>
        /// <param name="line"></param>
        /// <exception cref="">If a file could not be opened for write access</exception>
        public async Task WriteLineAsync(DateTime timestamp, string suffix, string line)
        {
            var filename = GetFilename(timestamp, suffix);
            if (!filename.Equals(CurrentFilename))
            {
                await CloseAsync();
                await OpenFileAsync(filename);
            }
            await Sw.WriteLineAsync(line);
            await Sw.FlushAsync();
        }

        public void Close()
        {
            if (Sw != null)
            {
                try
                {
                    Sw.Flush();
                    Sw.Close();
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex.ToString());
                }
            }

            Sw = null;
            CurrentFilename = null;

        }

        public async Task CloseAsync()
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

        protected virtual void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    // release managed resources
                    Close();
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
            Dispose(true);
            GC.SuppressFinalize(this);

        }
    }
}
