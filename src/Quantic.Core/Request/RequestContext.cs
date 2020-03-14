using System;
using System.Collections.Generic;

namespace Quantic.Core
{
    public class RequestContext
    {
        public RequestContext(string traceId,
            IDictionary<string, string> headers)
        {
            TraceId = string.IsNullOrEmpty(traceId)
                ? throw new ArgumentNullException(nameof(traceId))
                : traceId;

            Headers = headers ?? throw new ArgumentNullException(nameof(headers));
        }

        public string TraceId { get; }
        public IDictionary<string, string> Headers { get; }
        public string AcceptLanguage
        {
            get
            {
                string value;
                Headers.TryGetValue("Accept-Language", out value); 
                return value;                  
            }
        }
    }
}
