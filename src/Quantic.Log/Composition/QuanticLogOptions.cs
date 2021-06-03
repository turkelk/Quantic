using System;
using Microsoft.Extensions.Options;

namespace Quantic.Log
{
    public class QuanticLogOptions
    {
        internal Type RequestLoggerType { get; private set; } = null;



        public void AddRequestLogger<TLogger>()
            where TLogger : IRequestLogger
        {
            RequestLoggerType = typeof(TLogger);
        }
        public LogSettings LogSettings { get; private set; }

        public void AddLogSettings(LogSettings logSettings)
        {
            this.LogSettings = logSettings;
        }
    }
}
