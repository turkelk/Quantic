using System.Collections.Generic;
using Quantic.Core;

namespace Quantic.Validation.UnitTest
{
    public static class Helper
    {
        public static RequestContext Context = new RequestContext("trace-id", new Dictionary<string,string> { } );
    }
}