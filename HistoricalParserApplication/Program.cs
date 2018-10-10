using InfrastructureEf;
using InfrastructureWeb;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace HistoricalParserApplication
{
    internal class Program
    {
        static void Main(string[] args)
        {
            new ApplicationBuilder()
                .UseArguments(args)
                .UseConfiguration(Configuration)
                .Use<ApplicationStartup>()
                .Use<EfInfrastructureStartup>()
                .Use<WebInfrastructureStartup>()
                .Build()
                .Run();
        }


        private static IConfiguration Configuration => new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appconfig.json", optional: false)
            .AddJsonFile("appconfig.Packages.json", optional: false)
            .AddJsonFile("appconfig.InfrastructureEf.json", optional: false)
            .AddJsonFile("appconfig.InfrastructureWeb.json", optional: false)
            .Build();
    }
}
