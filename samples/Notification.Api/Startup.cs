using GreenPipes;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quantic.Cache.InMemory;
using Quantic.Core;
using Quantic.Log;
using Quantic.MassTransit.RabbitMq;
using Quantic.Validation;

namespace Notification.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }
        public void ConfigureServices(IServiceCollection services)
        {
            var assemblies = new System.Reflection.Assembly[] { 
                typeof(Startup).Assembly 
            };
            
            services.AddQuantic(cfg=> {
                cfg.Assemblies = assemblies;
            })
            .AddMemoryCacheDecorator()
            .AddValidationDecorator()
            .AddLogDecorator()
            .AddQuanticMassTransit((provider, cfg) => {
                cfg.ReceiveEndpoint("notification", endpoint => {
                    endpoint.Consumer<UserRegisteredConsumer>(provider);
                    endpoint.UseMessageRetry(x => x.Interval(2, 100));                 
                });                                          
            });

            string connectionString = Configuration.GetValue<string>("CONNECTION_STRING");            
            services.AddDbContext<DataContext>(options =>
            {
                options.UseMySql(connectionString);
            });

            services.AddHealthChecks()
                .AddMySql(connectionString, "MySQL")
                .AddRabbitMQ(name: "RabbitMQ");                       
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseHealthChecks("/hc");

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
