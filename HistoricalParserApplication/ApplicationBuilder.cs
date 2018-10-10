using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace HistoricalParserApplication
{
    internal class ApplicationBuilder
    {
        private string[] argv;
        private IConfiguration config;
        public ApplicationBuilder()
        {
        }

        public ApplicationBuilder UseArguments(string[] argv)
        {
            this.argv = argv;
            return this;
        }

        public ApplicationBuilder UseConfiguration(IConfiguration config)
        {
            this.config = config;
            return this;
        }

        public Application Build()
        {
            if (config == null) throw new ArgumentNullException("No configuration provided");

            var container = new ServiceCollection();
            ConfigureServices(container);

            var services = container.BuildServiceProvider();
            return services.GetService<Application>();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<Application>();
            /*services.AddTransient(
                (svc) => Log.Logger
            );*/
        }
    }
}
