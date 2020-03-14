using System;

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
    }
}
