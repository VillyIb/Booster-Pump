using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Microsoft.Extensions.Options;
using BoosterPumpConfiguration;
using eu.iamia.i2c.communication.contract;

namespace BoosterPumpLibrary.Logger
{
    [ExcludeFromCodeCoverage]
    // ReSharper disable once UnusedMember.Global
    public class OutputFileHandler : IOutputFileHandler, IDisposable
    {
        public DatabaseSettings Settings { get; }

        public OutputFileHandler(IOptions<DatabaseSettings> settings)
        {
            Settings = settings.Value;
        }

        private string? CurrentFilename { get; set; }

        private StreamWriter? Sw { get; set; }

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
            Sw = new(fs) { AutoFlush = true };

            fs.Position = fs.Seek(0, SeekOrigin.End);
            if (fs.Position == 0L)
            {
                Sw.WriteLine(Settings.Headline.Replace(';', SeparatorCharacter));
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

        [ExcludeFromCodeCoverage]
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

        [ExcludeFromCodeCoverage]
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);

        }
    }
}
