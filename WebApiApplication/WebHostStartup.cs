using Infrastructure;
using InfrastructureEf;
using InfrastructureEf.Repositories;
using InfrastructureWeb;
using InfrastructureWeb.Repositories;
using LibraryInitializer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Exceptions;
using System;
using WebApiApplication.AspInfrastructure.HostingServices;
using WebApiApplication.ScheduledTasks;

namespace WebApiApplication
{
    public class WebHostStartup
    {
        private readonly Startupper libraries;
        private readonly IConfiguration config;
        public WebHostStartup(IConfiguration config)
        {
            this.config = config;
            this.libraries = new Startupper();

            libraries.UseConfiguration(config);
            libraries.UseInitializer<EfInfrastructureStartup>();
            libraries.UseInitializer<WebInfrastructureStartup>();
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
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

            services.AddTransient(
                (svc) => Log.Logger
            );
            services.AddLogging( // redirect default Microsoft Logger to Serilog
                builder => builder.AddSerilog(dispose: true)
            );


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddScoped<IPairRepository, EfPairRepository>();

            ConfigureScheduledTasks(services);

            libraries.UseLogger(Log.Logger);
            libraries.ConfigureServices(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();

            libraries.Configure(serviceProvider);
        }


        private void ConfigureScheduledTasks(IServiceCollection services)
        {
            services.AddScheduler((sender, args) =>
            {
                Console.Write(args.Exception.Message);
                args.SetObserved();
            });

            services.AddSingleton<IScheduledTask, FetchRates>(
                (svc) =>
                {
                    return new FetchRates(config["RatesFetchSchedule"]);
                }
            );
        }
    }
}
