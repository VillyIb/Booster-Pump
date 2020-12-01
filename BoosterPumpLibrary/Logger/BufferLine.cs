using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace BoosterPumpLibrary.Logger
{
    public class BufferLine
    {
        public DateTime Timestamp { get; }

        public virtual string LogText { get; }

        public BufferLine(string logText, DateTime timestamp)
        {
            Timestamp = timestamp;
            LogText = logText;
        }

        public BufferLine(DateTime timestamp)
        {
            Timestamp = timestamp;
        }
    }

    public class BufferLineMeasurement : BufferLine
    {
        private static CultureInfo CultureInfo => CultureInfo.GetCultureInfo("da-DK"); //  da-DK

        public List<double> Values { get; }

        public BufferLineMeasurement(DateTime timestamp, params double[] values) : base(timestamp)
        {
            Values = values.ToList();
        }

        public override string LogText
        {
            get
            {
                var result = new StringBuilder();
                foreach (var value in Values)
                {
                    result.AppendFormat(CultureInfo, "{0:0000.0}", Math.Round(value, 1));
                }
                return result.ToString();
            }
        }
    }
}
