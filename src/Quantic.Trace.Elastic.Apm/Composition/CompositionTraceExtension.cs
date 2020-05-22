using Microsoft.Extensions.DependencyInjection;
using Quantic.Core;

namespace Quantic.Trace.Elastic.Apm
{
    public static class CompositionTraceExtension
    {
        public static IQuanticBuilder AddElasticApmDecorator(this IQuanticBuilder builder)
        {
            builder.Services.TryDecorate(typeof(ICommandHandler<>),typeof(TraceCommandHandlerDecorator<>));
            builder.Services.TryDecorate(typeof(IQueryHandler<,>), typeof(TraceQueryHandlerDecorator<,>));

            return builder;
        }
    }
}