﻿using System.Collections.Generic;
using Quantic.Core;

namespace Quantic.Cache.Redis.UnitTest
{
    public static class Util
    {
        public static RequestContext Context = new RequestContext("trace-id", new Dictionary<string, string>());
    }
}
