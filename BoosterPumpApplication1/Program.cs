using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BoosterPumpApplication;
using BoosterPumpLibrary.Logger;
using eu.iamia.Configuration;
using eu.iamia.NCD.API;
using eu.iamia.NCD.API.Contract;
using eu.iamia.ReliableSerialPort;

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
            var serialPort = scope.ServiceProvider.GetRequiredService<ISerialPortDecorator>();
            serialPort.Open();

            {
                var ncdCommand = new CommandControllerControllerBusSCan();
                var serialConverter = scope.ServiceProvider.GetRequiredService<IBridge>();
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

                if (!Console.KeyAvailable) continue;
                if (ConsoleKey.Q != Console.ReadKey(false).Key) continue;

                Console.WriteLine("Quit selected");
                loop = false;
            }

            logWriter.AggregateFlushUnconditionally();
        }
    }
}
