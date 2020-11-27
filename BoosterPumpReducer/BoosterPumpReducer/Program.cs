using System;
using System.IO;

namespace BoosterPumpReducer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var directory = new DirectoryInfo(@"C:\Users\Buzz Lightyear\Dropbox\_FlowMeasurement");

            // Open outputfile
            var outputFile = Path.Combine(directory.FullName, "AggregateFlow_27.txt");
            var outputfile = new OutputFile(outputFile);

            var aggregate = new AggregateData(outputfile);

            // Select Input files
            var filePrefix = @"FlowController_27";
            var readfile = new ReadFiles( directory, filePrefix, aggregate);

            // loop over input files
            readfile.Execute();         

        }
    }
}
