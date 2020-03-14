using System.Threading.Tasks;
using Quantic.Core;

namespace Quantic.Cache.InMemory.UnitTest
{
    [DecorateInMemoryCache(expireInSeconds:2, cacheKeyProviderType:typeof(FooCacheKeyProvider))]
    public class FooQueryHandler : IQueryHandler<FooQuery, string>
    {
        public Task<QueryResult<string>> Handle(FooQuery query, RequestContext context)
        {
            return Task.FromResult(new QueryResult<string>(query.ValueToCache));
        }
    }
}