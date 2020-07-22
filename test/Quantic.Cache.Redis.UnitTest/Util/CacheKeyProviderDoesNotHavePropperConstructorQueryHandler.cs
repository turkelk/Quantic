using System.Threading.Tasks;
using Quantic.Core;

namespace Quantic.Cache.Redis.UnitTest
{
    [DecorateRedisCache(cacheKeyProviderType:typeof(CacheKeyProviderWithoutPropperConstructor))]
    public class CacheKeyProviderDoesNotHavePropperConstructorQueryHandler : IQueryHandler<CacheKeyProviderDoesNotHavePropperConstructorQuery, string>
    {
        public Task<QueryResult<string>> Handle(CacheKeyProviderDoesNotHavePropperConstructorQuery query, RequestContext context)
        {
            return Task.FromResult(new QueryResult<string>("foo"));
        }
    }
}