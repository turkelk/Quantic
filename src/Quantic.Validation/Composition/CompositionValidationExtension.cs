using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Quantic.Core;

namespace Quantic.Validation
{
public static class CompositionValidationExtension
    {
        public static IQuanticBuilder AddValidationDecorator(this IQuanticBuilder builder, Action<QuanticValidationOptions> genesisOptions = null)
        {
            var options = new QuanticValidationOptions();
            genesisOptions?.Invoke(options);

            builder.Services.AddValidators(options.Assemblies ?? builder.Assemblies);  

            builder.Services.AddDecorators();          

            return builder;
        }

        private static IServiceCollection AddValidators(this IServiceCollection services, Assembly[] assemblies)
        {
            foreach(var asm in assemblies)
            {
                foreach(var type in asm.GetTypes())
                {
                    if(IsValidator(type))
                    {
                        services.AddTransient(typeof(QuanticValidator<>).MakeGenericType(type.BaseType.GetGenericArguments()), type);
                    }
                }    
            }

            static bool IsValidator(Type type)
            {
                return type.GetTypeInfo().IsAssignableTo(typeof(QuanticValidator<>).GetTypeInfo());
            }        

            return services;  
        }

        private static IServiceCollection AddDecorators(this IServiceCollection services)
        {
            foreach(var service in services.ToList())
            {
                if(ValidationEnabled(service)) 
                {
                    if(IsQueryHandler(service.ServiceType))
                    {
                        services.Decorate(service.ServiceType, typeof(ValidationQueryHandlerDecorator<,>).MakeGenericType(service.ServiceType.GenericTypeArguments));                
                    }

                    if(IsCommandHandler(service.ServiceType))
                    {
                        services.Decorate(service.ServiceType, typeof(ValidationCommandHandlerDecorator<>).MakeGenericType(service.ServiceType.GenericTypeArguments));                    
                    }
                }
            }
            
            static bool ValidationEnabled(ServiceDescriptor serviceDescriptor )
            {
                return serviceDescriptor.ImplementationType?.GetCustomAttribute<DecorateValidationAttribute>() != null;
            }

            static bool IsQueryHandler(Type type)
            {
                return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IQueryHandler<,>);
            }
            
            static bool IsCommandHandler(Type type)
            {
                return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ICommandHandler<>);
            }

            return services;
        }

        private static bool IsAssignableTo(this TypeInfo typeInfo, TypeInfo genericTypeInfo)
        {
            if (typeInfo.IsGenericType)
            {
                var typeDefinitionTypeInfo = typeInfo
                    .GetGenericTypeDefinition()
                    .GetTypeInfo();

                if (typeDefinitionTypeInfo.Equals(genericTypeInfo))
                {
                    return true;
                }
            }

            var baseTypeInfo = typeInfo.BaseType?.GetTypeInfo();

            return baseTypeInfo != null && baseTypeInfo.IsAssignableTo(genericTypeInfo);
        }
    }
}