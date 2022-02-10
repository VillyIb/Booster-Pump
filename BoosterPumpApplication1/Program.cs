using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BoosterPumpApplication;
using eu.iamia.Configuration;
using eu.iamia.NCD.API;
using eu.iamia.NCD.API.Contract;
using eu.iamia.SerialPortSetting.Contract;

namespace BoosterPumpApplication1
{
    [ExcludeFromCodeCoverage]
    public class Program
    {
        private static IConfiguration _configuration;

        // ReSharper disable once UnusedParameter.Local
        public static void Main(string[] args)
        {
            Console.WriteLine("Executing in SYNC mode");

            _configuration = ConfigurationSetup.Init();

            var setup = new Setup(_configuration);
            IServiceCollection services = new ServiceCollection();
            setup.Register(services);

            IServiceProvider serviceProvider = services.BuildServiceProvider();

            using var scope = serviceProvider.CreateScope();

            //var t1 = serviceProvider.GetService<IOptions<SerialPortSettings>>();
            //var t2 = t1.ToString();
            //var t3 = t1.Value;

            //var u1 =  serviceProvider.GetService<IOptions<ISerialPortSettings>>();


            //var serialPort = scope.ServiceProvider.GetRequiredService<ISerialPortDecorator>();
            //serialPort.Open();

            if (false)
                // ReSharper disable HeuristicUnreachableCode
            {
                var ncdCommand = new CommandControllerControllerHardReboot();
                var serialConverter = scope.ServiceProvider.GetRequiredService<IBridge>();
                var dataFromDevice = serialConverter.Execute(ncdCommand);
                if (!dataFromDevice.IsValid)
                {
                    throw new ApplicationException($"{ncdCommand.GetType().Name}: {dataFromDevice}");
                }
                Thread.Sleep(100);
            }
            // ReSharper restore HeuristicUnreachableCode

            {

            }

            {
                var ncdCommand = new CommandControllerControllerTest2WayCommunication();
                var serialConverter = scope.ServiceProvider.GetRequiredService<IBridge>();
                var dataFromDevice = serialConverter.Execute(ncdCommand);
                if (!dataFromDevice.IsValid)
                {
                    throw new ApplicationException($"{ncdCommand.GetType().Name}: {dataFromDevice}");
                }
            }
            {
                var ncdCommand = new CommandControllerControllerBusSCan();
                var serialConverter = scope.ServiceProvider.GetRequiredService<IBridge>();
                var dataFromDevice = serialConverter.Execute(ncdCommand);
                if (!dataFromDevice.IsValid)
                {
                    throw new ApplicationException($"{ncdCommand.GetType().Name}: {dataFromDevice}");
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
