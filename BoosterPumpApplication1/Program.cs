using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using System.Text;
using BoosterPumpApplication;
using BoosterPumpLibrary.Logger;
using eu.iamia.Configuration;
using NCD_API_SerialConverter;
using NCD_API_SerialConverter.Contracts;
using NCD_API_SerialConverter.NcdApiProtocol.SerialConverterCommands;

namespace BoosterPumpApplication1
{
    [ExcludeFromCodeCoverage]

    public class Program
    {

        private static CultureInfo CultureInfo => CultureInfo.GetCultureInfo("da-DK"); //  da-DK

        private static BufferedLogWriterV2 LogWriter { get; set; }

        private static IConfiguration Configuration;

        private static void Log(params object[] args)
        {
            var now = DateTime.UtcNow;
            now = new DateTime(
                now.Ticks - now.Ticks % (TimeSpan.TicksPerSecond / 10),
                now.Kind
            );

            var timestamp = now.ToLocalTime().ToString("O");
            var secondOfDay = (now.ToLocalTime().TimeOfDay.TotalMilliseconds / 100).ToString("000000", CultureInfo);

            var payload = new StringBuilder();

            foreach (var current in args)
            {
                payload.AppendFormat(CultureInfo, "{0:R}\t", current);
            }

            var row = $"{timestamp}\t{secondOfDay}\t{payload}";
            LogWriter.Add(row, now);

            Console.Write($"\r{row}");
        }

        // ReSharper disable once UnusedParameter.Local
        public static void Main(string[] args)
        {
            Console.WriteLine("Executing in SYNC mode");

            try
            {
                Configuration = ConfigurationSetup.Init();

                var setup = new Setup(Configuration);
                IServiceCollection services = new ServiceCollection();
                setup.Register(services);

                IServiceProvider serviceProvider = services.BuildServiceProvider();

                using var scope = serviceProvider.CreateScope();
                var serialPort = scope.ServiceProvider.GetRequiredService<INcdApiSerialPort>();
                serialPort.Open();

                {
                    var ncdCommand = new ConverterScan();
                    var serialConverter = scope.ServiceProvider.GetRequiredService<SerialConverter>();
                    var dataFromDevice = serialConverter.Execute(ncdCommand);
                    if (!(dataFromDevice.IsValid))
                    {
                        throw new ApplicationException(dataFromDevice.ToString());
                    }
                }

                var logWriter = scope.ServiceProvider.GetRequiredService<IBufferedLogWriter>();
                var controller = scope.ServiceProvider.GetRequiredService<IController>();

                var loop = true;
                while (loop)
                {
                    if (logWriter.IsNextMinute())
                    {
                        logWriter.AggregateFlush(DateTime.UtcNow);
                    }

                    controller.Execute(logWriter);

                    if (Console.KeyAvailable)
                    {
                        if (ConsoleKey.Q == Console.ReadKey(false).Key)
                        {
                            Console.WriteLine("Quit selected");
                            loop = false;
                        }
                    }
                }

                logWriter.AggregateFlushUnconditionally();
            }

            finally
            {

                LogWriter.Dispose();
            }
        }
    }
}
