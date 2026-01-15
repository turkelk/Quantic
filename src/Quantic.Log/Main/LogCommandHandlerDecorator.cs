using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quantic.Core;

namespace Quantic.Log
{
    public class LogCommandHandlerDecorator<TCommand> : ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        private const string ChannelHeader = "x-ba-channel";
        private readonly IRequestLogger requestLogger;
        private readonly ICommandHandler<TCommand> decoratedRequestHandler;
        private readonly ILogger<TCommand> logger;
        private readonly LogSettings logSettings;

        public LogCommandHandlerDecorator(IRequestLogger requestLogger,
            ICommandHandler<TCommand> decoratedRequestHandler,
            LogSettings logSettings,
            ILogger<TCommand> logger)
        {
            this.requestLogger = requestLogger;
            this.decoratedRequestHandler = decoratedRequestHandler;
            this.logger = logger;
            this.logSettings = logSettings;
        }

        public async Task<CommandResult> Handle(TCommand command, RequestContext context)
        {
            CommandResult result = null;
            DateTime requestDate = DateTime.UtcNow;

            using (logger.BeginScope($"trace-id:{context.TraceId}"))

                try
                {
                    result = await decoratedRequestHandler.Handle(command, context);
                }
                catch (Exception ex)
                {
                    result = new CommandResult(new Failure(Constants.UnhandledException, ex.ToString()));
                }
                finally
                {
                    var commandName = command.GetType().Name;
                    var logSetting = logSettings.Settings?.FirstOrDefault(x => x.Name == commandName);

                    if (logSetting == null || logSetting.ShouldLog)
                    {
                        // If response logging is disabled, keep a small summary.
                        var responseObj = (logSetting?.LogResponse ?? true)
                            ? (object)result
                            : new { result.Code, result.Errors, result.HasError, result.IsSuccess, result.Message, result.Retry };

                        await requestLogger.Log(new RequestLog
                        {
                            Name = commandName,
                            CorrelationId = context.TraceId,
                            Request = SanitizeRequest(logSetting, command),
                            RequestDate = requestDate,
                            Response = SanitizeResponse(logSetting, responseObj),
                            ResponseDate = DateTime.UtcNow,
                            Result = result.HasError ? Result.Error : Result.Success,
                            UserCode = context.UserId,
                            Channel = context.GetValue(ChannelHeader)
                        });
                    }
                }

            return result;
        }

        private object SanitizeRequest(LogSetting logSetting, object request)
        {
            if (request == null) return null;
            if (!(logSetting?.LogRequest ?? true)) return null;

            var props = (logSettings?.GlobalRedactProperties ?? Array.Empty<string>())
                .Concat(logSetting?.RedactRequestProperties ?? Array.Empty<string>());

            return LogRedactor.Redact(request, props, logSettings?.RedactionMask ?? "***");
        }

        private object SanitizeResponse(LogSetting logSetting, object response)
        {
            if (response == null) return null;

            var props = (logSettings?.GlobalRedactProperties ?? Array.Empty<string>())
                .Concat(logSetting?.RedactResponseProperties ?? Array.Empty<string>());

            return LogRedactor.Redact(response, props, logSettings?.RedactionMask ?? "***");
        }
    }
}
