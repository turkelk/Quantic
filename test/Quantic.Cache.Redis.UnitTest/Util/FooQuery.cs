using Quantic.Core;

namespace Quantic.Cache.Redis.UnitTest
{
    public class FooQuery : IQuery<string>
    {
        public string CacheKey { get; set; }
        public string ValueToCache {get;set;}
    }
}