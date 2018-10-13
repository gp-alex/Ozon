using Infrastructure;
using InfrastructureEf.Repositories;
using InfrastructureWeb.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Linq;

namespace HistoricalParserApplication
{
    public class ApplicationStartup
    {
        private readonly IConfiguration config;
        public ApplicationStartup(IConfiguration config)
        {
            this.config = config;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(
                (svc) => new Application(
                    config.GetSection("YearsToParse").GetChildren().Select(s => int.Parse(s.Value)),
                    svc.GetRequiredService<WebPairRepository>(),
                    svc.GetRequiredService<IPairRepository>(),
                    svc.GetService<ILogger>()
                )
            );


            services.AddTransient<IPairRepository, EfPairRepository>();
        }
    }
}
