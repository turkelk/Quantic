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
                return GetValue("Accept-Language");                                  
            }
        }

        public string UserId
        {
            get
            {
                return GetValue("X-User-Id");                  
            }
        } 

        public string GetValue(string key)
        {            
            string value = null;
            Headers?.TryGetValue(key, out value);
            return value;
        }

        public bool GetOrAdd(string key, string value)
        {
            return Headers?.TryAdd(key, value) ?? false;
        }        
    }
}
