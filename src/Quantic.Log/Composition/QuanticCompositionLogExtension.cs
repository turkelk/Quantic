using System;
using Microsoft.Extensions.DependencyInjection;
using Quantic.Core;

namespace Quantic.Log
{
    public static class QuanticCompositionLogExtension
    {
        public static IQuanticBuilder AddLogDecorator(this IQuanticBuilder builder, Action<QuanticLogOptions> logOptions = null)
        {
            var options = new QuanticLogOptions();
            logOptions?.Invoke(options);

            builder.Services.AddSingleton(options?.LogSettings ?? new LogSettings());
            builder.Services.AddSingleton(typeof(IRequestLogger), options.RequestLoggerType ?? typeof(RequestLogger));

            builder.Services.TryDecorate(typeof(ICommandHandler<>), typeof(LogCommandHandlerDecorator<>));
            builder.Services.TryDecorate(typeof(IQueryHandler<,>), typeof(LogQueryHandlerDecorator<,>));

            return builder;
        }
    }
}