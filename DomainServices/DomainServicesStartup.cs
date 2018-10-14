using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DomainServices
{
    public class DomainServicesStartup
    {
        private readonly IConfiguration config;
        public DomainServicesStartup(IConfiguration config)
        {
            this.config = config;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<WeeklyReportBuilder>();
            services.AddTransient<MonthlyReportBuilder>();
        }
    }
}
