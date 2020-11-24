namespace eu.iamia.Configuration
{
    using System;
    using Microsoft.Extensions.Configuration;

    public class ConfigurationSetup
    {
        public static IConfigurationRoot Init()
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var builder = new ConfigurationBuilder()
                    .AddJsonFile($"appsettings.json", true, true)
                    .AddJsonFile($"appsettings.{env}.json", true, true)
                    .AddEnvironmentVariables()
                ;

            var config = builder.Build();

            return config;
        }
    }
}
