using BoosterPumpLibrary.Logger;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using eu.iamia.Configuration;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

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
                var cs = scope.ServiceProvider.GetService<ControllerService>();

                var enabled = cs.Enabled;
                var minSpeedPct = cs.MinSpeedPct;
                var logWriter = scope.ServiceProvider.GetService<IBufferedLogWriter>();


                //var subdir = Configuration["Database:SubDirectory"];
                //var filePrefix = Configuration["Database:FilePrefix"];
                //var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                //var logfilePrefix = Path.Combine(userProfile, subdir, filePrefix);

                var tasks = new ConcurrentBag<Task>();

                //IOutputFileHandler outputFileHandler = new OutputFileHandler(logfilePrefix);

                using var tokenSource = new CancellationTokenSource();
                var token = tokenSource.Token;

                var logTask = logWriter.AggregateExecuteAsync(token);
                tasks.Add(logTask);

                var controller = new ControlAsync();

                var controlTask = controller.ExecuteAsync(token, logWriter);
                tasks.Add(controlTask);


                while (true)
                {
                    var input = Console.ReadLine();
                    if ("Q" == input || "q" == input)
                    {
                        tokenSource.Cancel();
                        try
                        {
                            await Task.WhenAll(tasks.ToArray());
                        }
                        catch (OperationCanceledException ex)
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
