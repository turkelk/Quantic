using System.Collections.Generic;
using Quantic.Core;

namespace Quantic.Log.UnitTest
{
    public static class Helper
    {
        public static RequestContext Context
        {
            get
            {
                string traceId = "trace_id";
                var headers = new Dictionary<string, string>()
                {
                };

                return new RequestContext(traceId, headers);
            }
        }

        public static string TestExceptionMessage = "TestExceptionMessage";
    }
}