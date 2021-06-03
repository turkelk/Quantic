using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quantic.Core;

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
            LogSettings logSettings,
            ILogger<TQuery> logger)
        {
            this.requestLogger = requestLogger;
            this.decoratedRequestHandler = decoratedRequestHandler;
            this.logger = logger;
            this.logSettings = logSettings;
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
                    var logSetting = logSettings.Settings?.FirstOrDefault(x => x.Name == queryName);

                    if (logSetting == null || logSetting.ShouldLog)
                    {
                        await requestLogger.Log(new RequestLog
                        {
                            Name = queryName,
                            CorrelationId = context.TraceId,
                            Request = logSetting?.LogRequest ?? true ? query : null,
                            RequestDate = requestDate,
                            Response = logSetting?.LogResponse ?? true ? result : new { result.Code, result.Errors, result.HasError, result.IsSuccess, result.Message, result.Retry },
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
