using System.Threading.Tasks;
using Quantic.Core;

namespace Quantic.Cache.InMemory.UnitTest
{
    public class BarQueryHandler : IQueryHandler<BarQuery, string>
    {
        public Task<QueryResult<string>> Handle(BarQuery query, RequestContext context)
        {
            return Task.FromResult(new QueryResult<string>("bar"));
        }
    }
}