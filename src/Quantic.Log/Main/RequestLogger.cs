using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Quantic.Log
{
    internal class RequestLogger : IRequestLogger
    {
        private readonly ILogger<RequestLogger> logger;

        public RequestLogger(ILogger<RequestLogger> logger)
        {
            this.logger = logger;
        }

        public async Task Log(RequestLog log)
        {
            log.Duration = (int)(log.ResponseDate - log.RequestDate).TotalMilliseconds;

            using (var stream = new MemoryStream())
            {
                await JsonSerializer.SerializeAsync<RequestLog>(stream, log);
                stream.Position = 0;
                using var reader = new StreamReader(stream);
                logger.Log(Microsoft.Extensions.Logging.LogLevel.Information, await reader.ReadToEndAsync());
            }
        }
    }
}
