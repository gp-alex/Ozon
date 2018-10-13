using LibraryInitializer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Exceptions;
using System;

namespace HistoricalParserApplication
{
    internal class ApplicationBuilder
    {
        private string[] argv;
        private IConfiguration config;
        private ILogger log;
        private readonly IServiceCollection serviceContainer;
        private readonly Startupper libraries;
        public ApplicationBuilder()
        {
            this.serviceContainer = new ServiceCollection();
            this.libraries = new Startupper();
        }

        public ApplicationBuilder UseArguments(string[] argv)
        {
            this.argv = argv;
            return this;
        }

        public ApplicationBuilder UseConfiguration(IConfiguration config)
        {
            this.config = config;
            this.libraries.UseConfiguration(config);
            return this;
        }

        public ApplicationBuilder Use<T>() where T : class
        {
            this.libraries.UseInitializer(typeof(T));
            return this;
        }

        public Application Build()
        {
            if (config == null) throw new ArgumentNullException("No configuration provided");

            AddCoreServices(config, serviceContainer);
            libraries.UseLogger(Log.Logger);
            libraries.ConfigureServices(serviceContainer);

            var services = serviceContainer.BuildServiceProvider();
            libraries.Configure(services);

            return services.GetService<Application>();
        }

        private void AddCoreServices(IConfiguration config, IServiceCollection services)
        {
#if DEBUG
            Serilog.Debugging.SelfLog.Enable(
                msg =>
                {
                    System.Diagnostics.Debug.WriteLine(msg);
                    System.Console.WriteLine(msg);
                }
            );
#endif
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(config.GetSection("Packages"))
                .Enrich.WithExceptionDetails()
                .CreateLogger();

            this.log = Log.Logger.ForContext(GetType());
            services.AddTransient(
                (svc) => Log.Logger
            );
            services.AddLogging( // redirect default Microsoft Logger to Serilog
                builder => builder.AddSerilog(dispose: true)
            );
        }
    }
}
