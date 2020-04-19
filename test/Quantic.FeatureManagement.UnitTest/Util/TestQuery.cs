using System.Threading.Tasks;
using Quantic.Core;

namespace Quantic.FeatureManagement.UnitTest
{
    [DecorateFeatureManagement("FeatureQ")]
    public class TestQueryHandler : IQueryHandler<TestQuery, string>
    {
        public Task<QueryResult<string>> Handle(TestQuery query, RequestContext context)
        {
            return Task.FromResult(QueryResult<string>.WithResult("OK"));
        }
    }
    public class TestQuery : IQuery<string> { }
}