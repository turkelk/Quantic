using System;
using Quantic.Core;

namespace Quantic.Web
{
    public static class CompositionWebExtension
    {
        public static IQuanticBuilder AddValidationDecorator(this IQuanticBuilder builder, Action<QuanticWebOptions> genesisOptions = null)
        {
            var options = new QuanticWebOptions();
            genesisOptions?.Invoke(options);

            return builder;
        }             
    }
}