using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BoosterPumpConfiguration;
using BoosterPumpLibrary.Logger;
using System.Diagnostics.CodeAnalysis;
using eu.iamia.NCD.API.Contract;
using eu.iamia.NCD.Bridge;
using eu.iamia.NCD.Serial;
using eu.iamia.ReliableSerialPort;
using eu.iamia.SerialPortSetting.Contract;
using Modules.AMS5812;
using Modules.AS1115;
using Modules.LPS25HB;
using Modules.MCP4725;
using Modules.TCA9546A;

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

            services.AddTransient(typeof(ISerialPortSettings), typeof(SerialPortSettings));

            services.AddSingleton(typeof(IBridge), typeof(ApiToSerialBridge));

            services.AddSingleton(typeof(IGateway), typeof(SerialGateway));
            services.AddSingleton(typeof(ISerialPortDecorator), typeof(SerialPortDecorator));

            services.AddSingleton(typeof(IOutputFileHandler), typeof(OutputFileHandler));
            services.AddSingleton(typeof(IBufferedLogWriter), typeof(BufferedLogWriterV2));


            services.AddTransient(typeof(IController), typeof(Controller));

            services.AddTransient(typeof(As1115Module), typeof(As1115Module));
            services.AddTransient(typeof(AMS5812_0150_D_Pressure), typeof(AMS5812_0150_D_Pressure));
            services.AddTransient(typeof(AMS5812_0300_A_Pressure), typeof(AMS5812_0300_A_Pressure));

            services.AddTransient(typeof(LPS25HB_Barometer), typeof(LPS25HB_Barometer));

            services.AddTransient(typeof(TCA9546A_Multiplexer), typeof(TCA9546A_Multiplexer));
            services.AddTransient(typeof(MCP4725_4_20mA_CurrentTransmitterV2), typeof(MCP4725_4_20mA_CurrentTransmitterV2));
        }
    }
}
