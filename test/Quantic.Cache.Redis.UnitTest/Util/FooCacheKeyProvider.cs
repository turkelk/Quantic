
using Quantic.Core;

namespace Quantic.Cache.Redis.UnitTest
{
    public class FooCacheKeyProvider : ICacheKeyProvider
    {
        private readonly FooQuery query;

        public FooCacheKeyProvider(FooQuery query, RequestContext context)
        {
            this.query = query;
        }
        public string GetKey()
        {
           return query.CacheKey;
        }
    }
}