using System;
using System.IO;

namespace BoosterPumpReducer
{
    class Program
    {
        static void Main(string[] args)
        {
            var date = DateTime.Now.AddDays(-1).Day.ToString("00");

            if (args.Length == 1)
            {
                int num;
                if (int.TryParse(args[0], out num))
                {
                    date = num.ToString("00");
                }
            }

            Console.WriteLine($"Compacting data for date: {date}");
              
            var directory = new DirectoryInfo(@"C:\Users\Buzz Lightyear\Dropbox\_FlowMeasurement");

            // Open outputfile
            var outputFile = Path.Combine(directory.FullName, $"AggregateFlow_{date}.txt");
            var outputfile = new OutputFile(outputFile);

            var aggregate = new AggregateData(outputfile);

            // Select Input files
            var filePrefix = $"FlowController_{date}";
            var readfile = new ReadFiles(directory, filePrefix, aggregate);

            // loop over input files
            readfile.Execute();

            Console.WriteLine($"Processed {readfile.Filecount} files");

        }
    }
}
