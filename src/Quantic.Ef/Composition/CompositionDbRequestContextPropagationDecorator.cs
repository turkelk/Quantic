using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Quantic.Core;
using Quantic.Ef.Main;

namespace Quantic.Ef.Composition
{
    public static class CompositionDbRequestContextPropagationDecorator
    {
        public static IQuanticBuilder AddDbRequestContextPropagationDecorator(this IQuanticBuilder builder, Action<QuanticEfConfig> efOptions = null)
        {
            var options = new QuanticEfConfig();
            efOptions?.Invoke(options);

            if(options.DbContextType == null) 
                throw new DbContextTypeMissingException();

            builder.Services.AddSingleton<QuanticEfConfig>(options);

            foreach (var service in builder.Services.ToList().Where(x => IsCommandHandler(x.ServiceType)))
            {
                builder.Services.Decorate(service.ServiceType, typeof(DbRequestContextPropagationDecorator<>)
                    .MakeGenericType(service.ServiceType.GenericTypeArguments));
            }

            static bool IsCommandHandler(Type type)
            {
                return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ICommandHandler<>);
            }

            return builder;
        }
    }
}