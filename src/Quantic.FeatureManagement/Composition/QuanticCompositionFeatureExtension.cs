using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Quantic.Core;

namespace Quantic.FeatureManagement
{
    public static class QuanticCompositionFeatureExtension
    {
        public static IQuanticBuilder AddFeatureManagementDecorator(this IQuanticBuilder builder)
        {
            builder.Services.AddSingleton<IHandlerFeatureInfoProvider, HandlerFeatureInfoProvider>();

            foreach (var service in builder.Services.ToList())
            {
                if (ShouldDecorateCommandHandler(service))
                {
                    builder.Services.Decorate(service.ServiceType, typeof(FeatureManagementCommandHandlerDecorator<>)
                        .MakeGenericType(service.ServiceType.GenericTypeArguments));
                }

                if (ShouldDecorateQueryHandler(service))
                {
                    builder.Services.Decorate(service.ServiceType, typeof(FeatureManagementQueryHandlerDecorator<,>)
                        .MakeGenericType(service.ServiceType.GenericTypeArguments));
                }

            }
            bool ShouldDecorateCommandHandler(ServiceDescriptor serviceDescriptor)
            {
                return serviceDescriptor.ServiceType.IsGenericType
                    && serviceDescriptor.ImplementationType?.GetCustomAttributes<DecorateFeatureManagementAttribute>()!= null
                    && serviceDescriptor.ImplementationType.GetCustomAttributes<DecorateFeatureManagementAttribute>().Any()
                    && serviceDescriptor.ServiceType.GetGenericTypeDefinition() == typeof(ICommandHandler<>);
            }

            bool ShouldDecorateQueryHandler(ServiceDescriptor serviceDescriptor)
            {
                return serviceDescriptor.ServiceType.IsGenericType
                    && serviceDescriptor.ImplementationType?.GetCustomAttributes<DecorateFeatureManagementAttribute>()!= null
                    && serviceDescriptor.ImplementationType.GetCustomAttributes<DecorateFeatureManagementAttribute>().Any()
                    && serviceDescriptor.ServiceType.GetGenericTypeDefinition() == typeof(IQueryHandler<,>);
            }

            return builder;
        }
    }
}