using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Quantic.Core;

namespace Quantic.Cache.InMemory
{
    public static class CompositionInMemoryCacheExtension
    {
        public static IQuanticBuilder AddMemoryCacheDecorator(this IQuanticBuilder builder, Action<InMemoryCacheOption> memoryCacheOption = null)
        {
            var options = new InMemoryCacheOption();
            memoryCacheOption?.Invoke(options);

            var cacheOptions = options?.MemoryCacheOptions ?? new MemoryCacheOptions();

            builder.Services.AddSingleton<IMemoryCache>(new MemoryCache(cacheOptions));

            List<Type> types = new List<Type>();

            foreach(var asm in builder.Assemblies)
            {
                types.AddRange(asm.GetTypes());
            }

            builder.Services.AddSingleton<IQueryInfoProvider>(new QueryInfoProvider(types));            

            foreach(var service in builder.Services.ToList().Where(x=> ShouldDecorate(x)))
            {
                builder.Services.Decorate(service.ServiceType, typeof(InMemoryCacheQueryHandlerDecorator<,>)
                    .MakeGenericType(service.ServiceType.GenericTypeArguments));
            }

            static bool ShouldDecorate(ServiceDescriptor serviceDescriptor)
            {
                return serviceDescriptor.ServiceType.IsGenericType 
                    && serviceDescriptor.ServiceType.GetGenericTypeDefinition() == typeof(IQueryHandler<,>)
                    && serviceDescriptor.ImplementationType?.GetCustomAttribute<DecorateInMemoryCacheAttribute>()!= null;
            }   

            return builder;
        }
    }
}
