using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Redis;
using Microsoft.Extensions.DependencyInjection;
using Quantic.Core;

namespace Quantic.Cache.Redis
{
    public static class CompositionRedisCacheExtension
    {
        public static IQuanticBuilder AddRedisCacheDecorator(this IQuanticBuilder builder, Action<RedisCacheOption> redisCacheOption = null)
        {
            var options = new RedisCacheOption();
            redisCacheOption?.Invoke(options);

            var cacheOptions = options?.RedisCacheOptions ?? new RedisCacheOptions();

            builder.Services.AddSingleton<IDistributedCache>(new RedisCache(cacheOptions));

            builder.Services.AddSingleton<IQueryInfoProvider,QueryInfoProvider>();            

            foreach(var service in builder.Services.ToList().Where(x=> ShouldDecorate(x)))
            {
                builder.Services.Decorate(service.ServiceType, typeof(RedisCacheQueryHandlerDecorator<,>)
                    .MakeGenericType(service.ServiceType.GenericTypeArguments));
            }

            static bool ShouldDecorate(ServiceDescriptor serviceDescriptor)
            {
                return serviceDescriptor.ServiceType.IsGenericType 
                    && serviceDescriptor.ServiceType.GetGenericTypeDefinition() == typeof(IQueryHandler<,>)
                    && serviceDescriptor.ImplementationType?.GetCustomAttribute<DecorateRedisCacheAttribute>()!= null;
            }   

            return builder;
        }
    }
}