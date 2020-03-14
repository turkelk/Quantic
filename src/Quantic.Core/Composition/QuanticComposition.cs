using System;
using Microsoft.Extensions.DependencyInjection;

namespace Quantic.Core
{
    public static class QuanticComposition
    {
        public static IQuanticBuilder AddQuantic(this IServiceCollection services, Action<QuanticOptions> genesisOptions)
        {
            var options = new QuanticOptions();
            genesisOptions?.Invoke(options);

            if (options.Assemblies == null)
                throw new ArgumentException("Assemblies cannot be null. It is reqiured to register command and query handlers", nameof(options.Assemblies));
            
            var builder = new QuanticBuilder(services, options.Assemblies);

            builder.Services.Scan(x => x.FromAssemblies(options.Assemblies)
                .AddClasses(classes => classes
                .WithoutAttribute<SkipRegisterAttribute>()            
                .AssignableTo(typeof(ICommandHandler<>)))
                .AsImplementedInterfaces()
                .WithTransientLifetime());


            builder.Services.Scan(x => x.FromAssemblies(options.Assemblies)
                .AddClasses(classes => classes
                .WithoutAttribute<SkipRegisterAttribute>() 
                .AssignableTo(typeof(IQueryHandler<,>)))
                .AsImplementedInterfaces()
                .WithTransientLifetime());              

            return builder;
        }
    }
}