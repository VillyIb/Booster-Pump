using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using eu.iamia.Configuration;
using BoosterPumpLibrary.Logger;
using BoosterPumpApplication;
using NCD_API_SerialConverter;
using NCD_API_SerialConverter.Contracts;
using NCD_API_SerialConverter.NcdApiProtocol.SerialConverterCommands;

namespace BoosterPumpApplicationAsync
{
    [ExcludeFromCodeCoverage]
    public class Program
    {
        private static IConfiguration Configuration;

        private static async Task ExecuteLoggerAsync(IBufferedLogWriter logWriter, CancellationToken cancellationToken)
        {
            try
            {
                Console.WriteLine("\r\n+BufferedLogWriterAsync.ExecuteAsync");
                do
                {
                    await logWriter.WaitUntilSecond02InNextMinuteAsync();
                    await logWriter.AggregateFlushAsync(DateTime.UtcNow);
                }
                while (!cancellationToken.IsCancellationRequested);

                await logWriter.AggregateFlushUnconditionalAsync();
            }
            catch (Exception ex)
            {
                await Console.Error.WriteLineAsync(ex.ToString());
            }
        }

        private static async Task ExecuteControllerAsync(IController controller, IBufferedLogWriter logWriter, IServiceScope scope, CancellationToken cancellationToken)
        {
            var stopwatch = new Stopwatch();
            try
            {
                Console.WriteLine("\r\n+BufferedLogWriterAsync.ExecuteAsync");

                controller.Init(scope);
                do
                {
                    stopwatch.Reset();
                    stopwatch.Start();

                    controller.Execute(logWriter);

                    stopwatch.Stop();
                    var duration = (int)stopwatch.ElapsedMilliseconds;
                    if (duration < controller.MeasurementSettings.RoundTripTime)
                    {
                        await Task.Delay(controller.MeasurementSettings.RoundTripTime - duration, cancellationToken);
                    }
                }
                while (!cancellationToken.IsCancellationRequested);
            }
            catch (Exception ex)
            {
                await Console.Error.WriteLineAsync(ex.ToString());
            }
        }

        public static async Task Main(string[] args)
        {
            Console.WriteLine("Executing in ASync mode");

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

            var tasks = new ConcurrentBag<Task>();

            using var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;

            var logTask = ExecuteLoggerAsync(logWriter, token);
            tasks.Add(logTask);

            var controlTask = ExecuteControllerAsync(controller, logWriter, scope, token);
            tasks.Add(controlTask);

            while (true)
            {
                var input = Console.ReadLine();
                if ("Q" == input || "q" == input)
                {
                    Console.WriteLine("\r\nClosing application, please wait");
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

            Console.WriteLine("Terminated");
        }
    }
}
