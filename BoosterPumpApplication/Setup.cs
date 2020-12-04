using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BoosterPumpLibrary.Contracts;
using NCD_API_SerialConverter;
using NCD_API_SerialConverter.Contracts;
using BoosterPumpConfiguration;
using BoosterPumpLibrary.Logger;
using System.Diagnostics.CodeAnalysis;
using Modules;

namespace BoosterPumpApplication
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

            services.AddSingleton(typeof(ISerialConverter), typeof(SerialConverter));
            services.AddSingleton(typeof(INcdApiSerialPort), typeof(SerialPortDecorator));
            services.AddSingleton(typeof(IOutputFileHandler), typeof(OutputFileHandler));
            services.AddSingleton(typeof(IBufferedLogWriter), typeof(BufferedLogWriterAsync));

            services.AddTransient(typeof(IController), typeof(Controller));

            services.AddTransient(typeof(As1115Module), typeof(As1115Module));
            services.AddTransient(typeof(AMS5812_0150_D_B_Module), typeof(AMS5812_0150_D_B_Module));
            services.AddTransient(typeof(AMS5812_0300_A_PressureModule), typeof(AMS5812_0300_A_PressureModule));

            //services.a
            services.AddTransient(typeof(LPS25HB_BarometerModule), typeof(LPS25HB_BarometerModule));

            services.AddTransient(typeof(TCA9546MultiplexerModule), typeof(TCA9546MultiplexerModule));
            services.AddTransient(typeof(MCP4725_4_20mA_CurrentTransmitterV2), typeof(MCP4725_4_20mA_CurrentTransmitterV2));
        }
    }
}
