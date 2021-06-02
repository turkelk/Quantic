
using Quantic.Core;

namespace Quantic.Cache.InMemory.UnitTest
{
    public class FooCacheKeyProvider : ICacheKeyProvider
    {
        public string GetKey(object query, RequestContext context)
        {
            return ((FooQuery)query).CacheKey;
        }
    }
}