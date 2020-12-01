using System.Linq;
using System.Security.Claims;
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

                var missingHeaders = MandatoryHeadersHolder.Headers.Keys.Except(headers.Keys);

                if (missingHeaders.Count() > 0)
                    throw new HeaderMissingException($"Headers {string.Join(',', missingHeaders)} are mandatory but missing in http request headers ");

                requestContext = new RequestContext(headers[traceIdKey], headers);
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                
                if (!string.IsNullOrEmpty(userId))
                    requestContext.Add("X-User-Id", userId);

                return requestContext;
            }
        }
    }
}