using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace BoosterPumpReducer
{
    public class ReadFiles
    {
        private static CultureInfo CultureInfo => CultureInfo.GetCultureInfo("da-DK"); //  da-DK

        public DirectoryInfo Directory { get; }

        public int Filecount { get; private set; }

        public string FilePrefix { get; }
        public AggregateData Aggregate { get; }

        public ReadFiles(DirectoryInfo directory, string filePrefix, AggregateData aggregate)
        {
            Directory = directory;
            FilePrefix = filePrefix;
            Aggregate = aggregate;
            Filecount = 0;
        }

        private void ParseLine(string line)
        {
            if(line.StartsWith("Timestamp")) { return; }

            var chunks = line.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);

            try
            {
                var timestamp = DateTime.Parse(chunks[0]);
                var values = new List<Double>();

                for (int index = 1; index < 7; index++)
                {
                    var value = double.Parse(chunks[index], CultureInfo);
                    values.Add(value);
                }

                if(values[2]< 0.0D || values[3]< 0.0D) { return; }

                Aggregate.Add(timestamp, values[1], values[2], values[3], values[4], values[5]);

            }
            catch(Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
        }

        private void HandleFile(FileInfo file)
        {
            Filecount++;
            var fs = file.OpenRead();
            using var sr = new StreamReader(fs);

            string currentLine;
            while( !String.IsNullOrEmpty(currentLine = sr.ReadLine()))
            {
                ParseLine(currentLine);
            }            
        }

        public void Execute()
        {
            var files = Directory.GetFiles($"{FilePrefix}*.txt");
            foreach(var file in files)
            {
                HandleFile(file);
            }

            Aggregate.Close();
        }
    }
}
