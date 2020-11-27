using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace BoosterPumpReducer
{
    public class OutputFile : IOutputFile
    {
        private static CultureInfo CultureInfo => CultureInfo.GetCultureInfo("da-DK"); //  da-DK

        private StreamWriter Sw;

        public void CloseFile()
        {
            if (null != Sw)
            {
                Sw.Flush();
                Sw.Close();
            }
            Sw = null;
        }

        public void OpenFile(string filename)
        {
            var fi = new FileInfo(filename);
            if(fi.Exists)
            {
                fi.Delete();
            }
            var fs = fi.Open(FileMode.OpenOrCreate);
            Sw = new StreamWriter(fs);

            fs.Position = fs.Seek(0, SeekOrigin.End);
            if (fs.Position == 0L)
            {
                Sw.WriteLine("Timestamp\tSecond of day" +
                                        "\tPressure Manifold\tFlow NorthWest\tFlow SouthEast\tSystem Pressure" +
                                        "\tTBarometer 1\tBarometer 2\tTemperature1\tTemperature2"
                );
            }
        }

        public OutputFile(string outputFile)
        {
            OpenFile(outputFile);
        }

        public void WriteLine(DateTime timestampLocal, params double[] values)
        {
            var payload = new StringBuilder();
            foreach (var value in values)
            {
                var vr = Math.Round(value, 1);
                payload.AppendFormat(CultureInfo, "{0:0000.0}\t", vr);
            }

            var ps = payload.ToString();          

            Sw.WriteLine($"{timestampLocal:O}\t{timestampLocal.TimeOfDay.TotalMinutes:0000}\t{ps}");
        }
    }
}
