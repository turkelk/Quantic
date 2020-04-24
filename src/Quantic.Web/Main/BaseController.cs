using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Quantic.Core;

namespace Quantic.Web
{
    public class BaseController : ControllerBase
    {
        private const string traceIdKey = "quantic-trace-id";
        RequestContext requestContext;
        protected RequestContext Context
        {
            get
            {
                if (requestContext != null)
                    return requestContext;

                var headers = Request.Headers.ToDictionary(k => k.Key, v => v.Value.First());

                if (!headers.ContainsKey(traceIdKey))
                {
                    throw new TraceIdMissingExpcetion();
                }

                requestContext = new RequestContext(headers[traceIdKey], headers);
                return requestContext;
            }
        }
    }
}