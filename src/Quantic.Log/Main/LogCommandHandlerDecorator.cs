using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Quantic.Core;
using Quantic.Log.Util;

namespace Quantic.Log
{
    public class LogCommandHandlerDecorator<TCommand> : ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        
        private readonly IRequestLogger requestLogger;
        private readonly ICommandHandler<TCommand> decoratedRequestHandler;
        private readonly ILogger<TCommand> logger;
        private readonly LogSettings logSettings;

        public LogCommandHandlerDecorator(IRequestLogger requestLogger,
            ICommandHandler<TCommand> decoratedRequestHandler,
            IOptionsSnapshot<LogSettings> logSettingsOption,
            ILogger<TCommand> logger)
        {
            this.requestLogger = requestLogger;
            this.decoratedRequestHandler = decoratedRequestHandler;
            this.logger = logger;
            this.logSettings = logSettingsOption.Value;
        }

        public async Task<CommandResult> Handle(TCommand command, RequestContext context)
        {
            CommandResult result = null;
            DateTime requestDate = DateTime.UtcNow;
            
            using(logger.BeginScope($"trace-id:{context.TraceId}"))

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
                var commandName =  command.GetType().Name;
                bool shouldLog = logSettings?.ShouldLog(commandName) ?? true;
                
                if(shouldLog)
                {
                    requestLogger.Log(new RequestLog
                    {
                        Name = commandName,
                        CorrelationId = context.TraceId,
                        Request = command,
                        RequestDate = requestDate,
                        Response = result,
                        ResponseDate = DateTime.UtcNow,
                        Result = result.HasError ? Result.Error : Result.Success
                        //UserCode = context.CorrelationContext.UserId
                    });                   
                }
            }

            return result;
        }
    }
}
