using InfrastructureWeb.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace InfrastructureWeb
{
    public class WebInfrastructureStartup
    {
        private readonly IConfiguration config;
        public WebInfrastructureStartup(IConfiguration config)
        {
            this.config = config;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient();

            ConfigureCustomServices(services);
        }

        private void ConfigureCustomServices(IServiceCollection services)
        {
            services.AddTransient(
                (svc) => new WebPairRepository(
                    config["DailyRatesEndpoint"],
                    config["YearlyRatesEndpoint"],
                    svc.GetRequiredService<IHttpClientFactory>(),
                    svc.GetService<ILogger>()
                )
            );
        }
    }
}
