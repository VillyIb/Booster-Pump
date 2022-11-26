// ReSharper disable StringLiteralTypo
// ReSharper disable UnusedMember.Global
namespace eu.iamia.Configuration
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.Extensions.Configuration;

    [ExcludeFromCodeCoverage]
    public class ConfigurationSetup
    {
        public static IConfiguration Init()
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var builder = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", true, true) // Requires: Microsoft.Extensions.Configuration.Json
                    .AddJsonFile($"appsettings.{env}.json", true, true)
                    .AddEnvironmentVariables() // Requires: Microsoft.Extensions.Configuration.EnvironmentVariables
                ;

            IConfiguration config = builder.Build();

            return config;
        }
    }
}
