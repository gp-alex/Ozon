using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Threading.Tasks;

namespace WebApiApplication.AspInfrastructure.HostingServices
{
    public static class AspTaskSchedulerExtensions
    {
        public static IServiceCollection AddScheduler(this IServiceCollection services)
        {
            return services.AddSingleton<IHostedService, AspTaskScheduler>();
        }

        public static IServiceCollection AddScheduler(this IServiceCollection services, EventHandler<UnobservedTaskExceptionEventArgs> unobservedTaskExceptionHandler)
        {
            return services.AddSingleton<IHostedService, AspTaskScheduler>(serviceProvider =>
            {
                var instance = new AspTaskScheduler(
                    serviceProvider.GetServices<IScheduledTask>(),
                    serviceProvider.GetRequiredService<IServiceScopeFactory>(),
                    serviceProvider.GetService<ILogger>()
                );
                instance.UnobservedTaskException += unobservedTaskExceptionHandler;
                return instance;
            });
        }
    }
}
