using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using eu.iamia.Configuration;
using BoosterPumpLibrary.Logger;
using BoosterPumpApplication;
using BoosterPumpLibrary.Contracts;
using NCD_API_SerialConverter;
using NCD_API_SerialConverter.Contracts;
using NCD_API_SerialConverter.NcdApiProtocol.SerialConverterCommands;

namespace BoosterPumpApplicationAsync
{
    [ExcludeFromCodeCoverage]
    class Program
    {
        private static IConfiguration Configuration;

        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            Configuration = ConfigurationSetup.Init();
            IServiceCollection services = new ServiceCollection();
            var setup = new Setup(Configuration);
            setup.Register(services);

            IServiceProvider serviceProvider = services.BuildServiceProvider();

            using (var scope = serviceProvider.CreateScope())
            {
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

                var tasks = new ConcurrentBag<Task>();

                using var tokenSource = new CancellationTokenSource();
                var token = tokenSource.Token;

                var logTask = logWriter.AggregateExecuteAsync(token);
                tasks.Add(logTask);

                var controller = scope.ServiceProvider.GetRequiredService<IController>();

                var controlTask = controller.ExecuteAsync(token, logWriter);
                tasks.Add(controlTask);

                while (true)
                {
                    var input = Console.ReadLine();
                    if ("Q" == input || "q" == input)
                    {
                        Console.WriteLine($"\r\nClosing application, please wait");
                        tokenSource.Cancel();
                        try
                        {
                            await Task.WhenAll(tasks.ToArray());
                        }
                        catch (OperationCanceledException)
                        {
                            Console.WriteLine($"\n{nameof(OperationCanceledException)} thrown\n");
                        }
                        break;
                    }
                }
            }

            Console.WriteLine("Terminated");
        }
    }
}
