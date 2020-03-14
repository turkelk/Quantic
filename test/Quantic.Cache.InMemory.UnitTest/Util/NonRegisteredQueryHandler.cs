using System.Threading.Tasks;
using Quantic.Core;

namespace Quantic.Cache.InMemory.UnitTest
{
    public class NonRegisteredQueryHandler : IQueryHandler<NonRegisteredQuery, int>
    {
        public Task<QueryResult<int>> Handle(NonRegisteredQuery query, RequestContext context)
        {
            return Task.FromResult(new QueryResult<int>(0));
        }
    }
}