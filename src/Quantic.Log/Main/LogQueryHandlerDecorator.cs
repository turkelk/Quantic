using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quantic.Core;
using Quantic.Log.Util;

namespace Quantic.Log
{
    public class LogQueryHandlerDecorator<TQuery, TResponse> : IQueryHandler<TQuery, TResponse>
               where TQuery : IQuery<TResponse>
    {
        private readonly IRequestLogger requestLogger;
        private readonly IQueryHandler<TQuery, TResponse> decoratedRequestHandler;
        private readonly ILogger<TQuery> logger;
        private readonly LogSettings logSettings;

        public LogQueryHandlerDecorator(IRequestLogger requestLogger,
            IQueryHandler<TQuery, TResponse> decoratedRequestHandler,
            IOptionsSnapshot<LogSettings> logSettingsOption,
            ILogger<TQuery> logger)
        {
            this.requestLogger = requestLogger;
            this.decoratedRequestHandler = decoratedRequestHandler;
            this.logger = logger;
            this.logSettings = logSettingsOption.Value;
        }

        public async Task<QueryResult<TResponse>> Handle(TQuery query, RequestContext context)
        {
            QueryResult<TResponse> result = null;
            DateTime requestDate = DateTime.UtcNow;

            using (logger.BeginScope($"trace-id:{context.TraceId}"))

                try
                {
                    result = await decoratedRequestHandler.Handle(query, context);
                }
                catch (Exception ex)
                {
                    result = new QueryResult<TResponse>(new Failure(Constants.UnhandledException, ex.ToString()));
                }
                finally
                {
                    var queryName = query.GetType().Name;
                    bool shouldLog = logSettings?.ShouldLog(queryName) ?? true;

                    if (shouldLog)
                    {
                        await requestLogger.Log(new RequestLog
                        {
                            Name = queryName,
                            CorrelationId = context.TraceId,
                            Request = query,
                            RequestDate = requestDate,
                            Response = result,
                            ResponseDate = DateTime.UtcNow,
                            Result = result.HasError ? Result.Error : Result.Success,
                            UserCode = context.UserId
                        });
                    }
                }

            return result;
        }
    }
}
