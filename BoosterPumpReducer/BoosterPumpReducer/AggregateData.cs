using System;
using System.Linq;
using System.Collections.Generic;

namespace BoosterPumpReducer
{
    public class AggregateData
    {
        private const int DataColumns = 4;
        private readonly IOutputFile OutputFile;

        public static DateTime RoundToMinute(DateTime value)
        {
            return new DateTime(value.Ticks - value.Ticks % TimeSpan.TicksPerMinute, value.Kind);
        }

        private DateTime? currentTimestamp { get; set; }

        private List<Double> aggregatedValues { get; }
        private int aggregateCount { get; set; }

        public AggregateData(IOutputFile outputFile)
        {
            aggregatedValues = new List<double>();
            this.OutputFile = outputFile;
            currentTimestamp = null;
        }

        private void WriteAggregateDataToFile()
        {
            if(aggregatedValues.Any())
            {
                var t1 = aggregatedValues.Select(t => t / aggregateCount).ToList();                
                var ta = (Math.Log10( Math.Max(0.1, t1[1]))+1)*10;
                t1.Add(ta);
                var tb = (Math.Log10(Math.Max(0.1, t1[2]))+1) * 10;
                t1.Add(tb);
                var tc = ta + tb;
                t1.Add(tc);
                OutputFile.WriteLine(currentTimestamp.Value, t1.ToArray());
            }
            aggregatedValues.Clear();
            for(int index = 0; index < DataColumns; index++)
            {
                aggregatedValues.Add(0.0d);
            }
            aggregateCount = 0;
        }

        public void Close()
        {
            WriteAggregateDataToFile();
            OutputFile.CloseFile();
            aggregatedValues.Clear();
        }

        public void Add(DateTime timestamp, params double[] values)
        {
            var aggregatedTime = RoundToMinute(timestamp);

            if (! aggregatedTime.Equals(currentTimestamp) )
            {
                WriteAggregateDataToFile();
            }

            currentTimestamp = aggregatedTime;
            for(int index = 0; index < DataColumns; index++)
            {
                aggregatedValues[index] += values[index];
            }
            aggregateCount++;
        }
    }
}
