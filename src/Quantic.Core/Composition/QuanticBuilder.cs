using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Quantic.Core.Util;

namespace Quantic.Core
{
    public class QuanticBuilder : IQuanticBuilder
    {
        public QuanticBuilder(IServiceCollection services, params Assembly[] assemblies)
        {
            services.Guard(nameof(services));
            assemblies.Guard(nameof(assemblies));

            Services = services;
            Assemblies = assemblies;
        }
        public IServiceCollection Services { get; }

        public Assembly[] Assemblies { get; }
    }
}