using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Quantic.Core;

namespace Quantic.Web
{
public class BaseController : ControllerBase
{
        RequestContext requestContext;
        protected RequestContext Context
        {
            get
            {
                if (requestContext != null)
                    return requestContext;

                var headers = Request.Headers.ToDictionary(k => k.Key, v => v.Value.First());
                
                if(!headers.ContainsKey("quantic-trace-id"))
                {
                    throw new TraceIdMissingExpcetion();
                }  
                            
                requestContext = new RequestContext(headers["quantic-trace-id"], headers);
                return requestContext;
            }
        }

    }
}