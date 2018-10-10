using Infrastructure;
using InfrastructureEf.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InfrastructureEf
{
    public class EfInfrastructureStartup
    {
        private readonly IConfiguration config;
        public EfInfrastructureStartup(IConfiguration config)
        {
            this.config = config;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureCustomServices(services);
        }

        private void ConfigureCustomServices(IServiceCollection services)
        {
            services.AddTransient<EfPairRepository>();
        }
    }
}
