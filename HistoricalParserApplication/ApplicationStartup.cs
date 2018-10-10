using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
            services.AddSingleton<Application>();
        }
    }
}
