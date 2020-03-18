using GreenPipes;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Quantic.Cache.InMemory;
using Quantic.Core;
using Quantic.Log;
using Quantic.MassTransit.RabbitMq;
using Quantic.Validation;

namespace Identity.Api
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
                cfg.ReceiveEndpoint("identity", endpoint => {
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

            services.AddControllers();

            services.AddApiVersioning(o => {
                o.ReportApiVersions = true;
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.DefaultApiVersion = new ApiVersion(1, 0);
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Identity", Version = "v1" });
            });                     
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();
            
            app.UseSwaggerUI(c=> {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Identity V1");
            });

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
