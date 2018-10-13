using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LibraryInitializer
{
    public class Startupper
    {
        private IConfiguration config;
        private ILogger log;
        private readonly IDictionary<Type, object> initializers;
        public Startupper()
            : base()
        {
            this.initializers = new Dictionary<Type, object>();
        }

        public void UseConfiguration(IConfiguration config)
        {
            this.config = config;
        }

        public void UseLogger(ILogger log)
        {
            this.log = log;
        }

        public void UseInitializer<T>() where T : class
        {
            UseInitializer(typeof(T));
        }

        public void UseInitializer(Type type)
        {
            this.initializers.Add(type, null);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            foreach (var starterType in initializers.Keys.ToArray())
            {
                try
                {
                    var starterObject = GetInitializer(starterType);
                    var configureServices = starterType.GetMethod("ConfigureServices");
                    if (configureServices != null)
                    {
                        configureServices.Invoke(
                            starterObject,
                            new object[] { services }
                        );
                    }
                }
                catch (Exception e)
                {
                    log?.Warning(e, "ConfigureLibraryServices");
                }
            }
        }

        public void Configure(IServiceProvider services)
        {
            foreach (var starterType in initializers.Keys.ToArray())
            {
                try
                {
                    var starterObject = GetInitializer(starterType);
                    var configureMethod = starterType.GetMethod("Configure");
                    if (configureMethod != null)
                    {
                        var arguments = configureMethod.GetParameters();
                        var parameters = arguments
                            .OrderBy(x => x.Position)
                            .Select(
                                x => services.GetService(x.ParameterType)
                            )
                            .ToArray();

                        configureMethod.Invoke(
                            starterObject,
                            parameters
                        );
                    }
                }
                catch (Exception e)
                {
                    log?.Warning(e, "ConfigureLibraryServices");
                }
            }
        }

        private object GetInitializer(Type type)
        {
            if (initializers.ContainsKey(type))
            {
                if (initializers[type] == null)
                {
                    var initializerConfig = config.GetSection(type.Namespace);
                    initializers[type] = Activator.CreateInstance(
                        type,
                        new object[] { initializerConfig }
                    );
                }

                return initializers[type];
            }

            return null;
        }
    }
}
