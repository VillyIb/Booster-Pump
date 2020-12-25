using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
        private static IConfiguration Configuration;

        // ReSharper disable once UnusedParameter.Local
        public static void Main(string[] args)
        {
            Console.WriteLine("Executing in SYNC mode");

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
                if (!dataFromDevice.IsValid)
                {
                    throw new ApplicationException(dataFromDevice.ToString());
                }
            }

            var logWriter = scope.ServiceProvider.GetRequiredService<IBufferedLogWriter>();

            var controller = scope.ServiceProvider.GetRequiredService<IController>();
            controller.Init(scope);

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
    }
}
