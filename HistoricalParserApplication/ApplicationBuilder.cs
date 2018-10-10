using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HistoricalParserApplication
{
    internal class ApplicationBuilder
    {
        private string[] argv;
        private IConfiguration config;
        private ILogger log;
        private readonly IServiceCollection serviceContainer;
        private readonly ICollection<Type> startups;
        public ApplicationBuilder()
        {
            this.serviceContainer = new ServiceCollection();
            this.startups = new List<Type>();
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

        public ApplicationBuilder Use<T>() where T : class
        {
            var startup = typeof(T);
            if (startups.Where(t => t.Equals(startup)).SingleOrDefault() == null)
            {
                startups.Add(startup);
            }
            return this;
        }

        public Application Build()
        {
            if (config == null) throw new ArgumentNullException("No configuration provided");

            ConfigureCoreServices(config, serviceContainer);
            ConfigureLibraryServices(config, serviceContainer);

            var services = serviceContainer.BuildServiceProvider();
            return services.GetService<Application>();
        }

        private void ConfigureCoreServices(IConfiguration config, IServiceCollection services)
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
                .ReadFrom.Configuration(config)
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

        private void ConfigureLibraryServices(IConfiguration config, IServiceCollection services)
        {
            foreach (var startup in startups)
            {
                try
                {
                    var configureServices = startup.GetMethod("ConfigureServices");
                    if (configureServices != null)
                    {
                        var libraryConfig = config.GetSection(startup.Namespace);
                        configureServices.Invoke(
                            Activator.CreateInstance(
                                startup, new object[] { libraryConfig }
                            ),
                            new object[] { services }
                        );
                    }
                }
                catch (Exception e)
                {
                    log.Warning(e, "ConfigureLibraryServices");
                }
            }
        }
    }
}
