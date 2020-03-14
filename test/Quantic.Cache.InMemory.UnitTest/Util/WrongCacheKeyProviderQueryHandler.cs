using System.Threading.Tasks;
using Quantic.Core;

namespace Quantic.Cache.InMemory.UnitTest
{

    [DecorateInMemoryCache(cacheKeyProviderType:typeof(WrongCacheKeyProviderQuery))]
    public class WrongCacheKeyProviderQueryHandler : IQueryHandler<WrongCacheKeyProviderQuery, string>
    {
        public Task<QueryResult<string>> Handle(WrongCacheKeyProviderQuery query, RequestContext context)
        {
            return Task.FromResult(new QueryResult<string>("foo"));
        }
    }
}