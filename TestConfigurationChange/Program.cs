using System;
using System.Threading;
using eu.iamia.Configuration;
using Microsoft.Extensions.Configuration;

namespace TestConfigurationChange
{
    public class Measurements
    {
        public string RoundTripTime { get; set; }

        public string ManifoldPressureCorrection { get; set; }

        public string FlowNorthWestCorrection { get; set; }

        public string FlowSouthEastCorrection { get; set; }

        public string SystemPressureCorrection { get; set; }

    }

    class Program
    {
        private static IConfiguration Configuration;

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            Configuration = ConfigurationSetup.Init();

            var measurements = new Measurements();
            Configuration.GetSection("Measurements").Bind(measurements);

            var changeToken = Configuration.GetReloadToken();


            while (true)
            {
                Console.WriteLine();
                Console.WriteLine(Configuration["Measurements:ManifoldPressureCorrection"]);
                Console.WriteLine(measurements.ManifoldPressureCorrection);
                Console.WriteLine(Configuration.GetSection("Measurements").Get<Measurements>().ManifoldPressureCorrection);
                Thread.Sleep(8000);

                if (changeToken.HasChanged)
                {
                    
                }

                //Configuration.Reload();
            }
        }
    }
}
