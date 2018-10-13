using Infrastructure;
using InfrastructureEf.EntityFramework;
using InfrastructureEf.Repositories;
using Microsoft.EntityFrameworkCore;
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
            services.AddTransient<EfPairRepository>();

            ConfigureInternalServices(services);
        }

        private void ConfigureInternalServices(IServiceCollection services)
        {
            services.AddDbContext<SharedContext>(
                opt => opt.UseSqlite(
                    config["ConnectionStrings:SharedStorage"]
                )
            );
        }


        public void Configure(SharedContext context)
        {
            context.Database.EnsureCreated();
        }
    }
}
