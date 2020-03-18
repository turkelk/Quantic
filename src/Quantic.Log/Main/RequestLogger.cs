using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Quantic.Log
{
    internal class RequestLogger : IRequestLogger
    {
        private readonly ILogger<RequestLogger> logger;

        public RequestLogger(ILogger<RequestLogger> logger)
        {
            this.logger = logger;
        }

        public void Log(RequestLog log)
        {
            logger.Log(Microsoft.Extensions.Logging.LogLevel.Information, JsonConvert.SerializeObject(log,SerializerSettings.Value));
        }
    }
}
