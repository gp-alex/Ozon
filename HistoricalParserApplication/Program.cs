using InfrastructureEf;
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
                .Use<InfrastructureStartup>()
                .Build()
                .Run();
        }


        private static IConfiguration Configuration => new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appconfig.json", optional: false)
            .AddJsonFile("appconfig.Core.json", optional: false)
            .AddJsonFile("appconfig.Infrastructure.json", optional: false)
            .Build();
    }
}
