using System;
using MassTransit;
using MassTransit.RabbitMqTransport;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quantic.Core;
using RabbitMQ.Client;

namespace Quantic.MassTransit.RabbitMq
{
    public static class QuanticMasstransitRabbitMqExtension
    {
        public static IQuanticBuilder AddQuanticMassTransit(this IQuanticBuilder builder, 
            Action<IServiceProvider, IRabbitMqBusFactoryConfigurator> rabbitMqBusFactoryConfigurator,
            Action<QuanticMassTransitRabbitMqOptions> masstransitOptions = null)
        {       
            var option = new QuanticMassTransitRabbitMqOptions();
            masstransitOptions?.Invoke(option); 
            
            var rabbitConfig =  option.RabbitConfig ?? GetRabbitMqConfig(builder.Services);
            builder.Services.AddSingleton(rabbitConfig);

            var factory = new ConnectionFactory()
            {
                HostName = rabbitConfig.Host,
                Password = rabbitConfig.Password,
                UserName = rabbitConfig.UserName,
                VirtualHost = "/",
                Port = rabbitConfig.Port,
                AutomaticRecoveryEnabled = true
            };

            var connection = factory.CreateConnection();
            builder.Services.AddSingleton<IConnection>(connection);

            builder.Services.AddMassTransit(x => { 
                
                x.AddConsumers(option.ConsumerAssemblies ?? builder.Assemblies);

                x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg => {
                    Action<IRabbitMqHostConfigurator> configure = h =>
                    {
                        h.Username(rabbitConfig.UserName);
                        h.Password(rabbitConfig.Password);
                    };
                    
                    cfg.Host(new Uri($"rabbitmq://{rabbitConfig.Host}"),"/", configure);

                    rabbitMqBusFactoryConfigurator?.Invoke(provider, cfg);                  
                }));
            });  
            
            builder.Services.AddSingleton<IQuanticBus, QuanticBus>();
            builder.Services.AddSingleton<IHostedService, BusService>();             

            return builder;
        }

        static RabbitMqConfig GetRabbitMqConfig(IServiceCollection services)
        {
            using (var serviceProvider = services.BuildServiceProvider())
            {
                var configuration = serviceProvider.GetService<IConfiguration>();
                
                var config = new RabbitMqConfig();
                config.Host = configuration.GetValue<string>("RABBIT_HOST");
                config.UserName = configuration.GetValue<string>("RABBIT_USER_NAME");
                config.Password = configuration.GetValue<string>("RABBIT_PASSWORD");
                config.Port = configuration.GetValue<int>("RABBIT_PORT");

                return config;
            }
        }
    }
}