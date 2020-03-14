using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Quantic.Core
{
    public interface IQuanticBuilder
    {
        IServiceCollection Services { get; }
        Assembly[] Assemblies { get; }      
    }
}