using System.Threading.Tasks;
using Quantic.Core;

namespace Quantic.Cache.InMemory.UnitTest
{
    [DecorateInMemoryCache(cacheKeyProviderType:typeof(CacheKeyProviderWithoutPropperConstructor))]
    public class CacheKeyProviderDoesNotHavePropperConstructorQueryHandler : IQueryHandler<CacheKeyProviderDoesNotHavePropperConstructorQuery, string>
    {
        public Task<QueryResult<string>> Handle(CacheKeyProviderDoesNotHavePropperConstructorQuery query, RequestContext context)
        {
            return Task.FromResult(new QueryResult<string>("foo"));
        }
    }
}