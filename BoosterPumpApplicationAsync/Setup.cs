using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BoosterPumpLibrary.Contracts;
using NCD_API_SerialConverter;
using NCD_API_SerialConverter.Contracts;
using BoosterPumpConfiguration;
using BoosterPumpLibrary.Logger;
using System.Diagnostics.CodeAnalysis;

namespace BoosterPumpApplicationAsync
{
    [ExcludeFromCodeCoverage]
    public class Setup
    {
        public IConfiguration Configuration { get; }

        public Setup(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        public void Register(IServiceCollection services)
        {
            // NB! Configure(...) requires NuGet package Microsoft.Extensions.Options.ConfigurationExtensions

            services.Configure<SerialPortSettings>(Configuration.GetSection(SerialPortSettings.Name));
            services.Configure<DatabaseSettings>(Configuration.GetSection(DatabaseSettings.Name));
            services.Configure<MeasurementSettings>(Configuration.GetSection(MeasurementSettings.Name));
            services.Configure<ControllerSettings>(Configuration.GetSection(ControllerSettings.Name));
            services.Configure<AlarmSettings>(Configuration.GetSection(AlarmSettings.Name));

            services.AddOptions();

            services.AddTransient<ControllerService, ControllerService>();

            services.AddSingleton(typeof(ISerialConverter), typeof(SerialConverter));
            services.AddSingleton(typeof(INcdApiSerialPort), typeof(SerialPortDecorator));
            services.AddSingleton(typeof(IOutputFileHandler), typeof(OutputFileHandler));
            services.AddSingleton(typeof(IOutputFileHandler), typeof(OutputFileHandler));

            //services.AddSingleton(typeof(IOutputFileHandler), typeof(OutputFileHandler));
            //services.AddSingleton(typeof(IOutputFileHandler), typeof(OutputFileHandler));
            //services.AddSingleton(typeof(IOutputFileHandler), typeof(OutputFileHandler));


        }
    }
}
