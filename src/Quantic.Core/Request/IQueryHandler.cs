using System.Threading.Tasks;

namespace Quantic.Core
{
    public interface IQueryHandler<TQuery, TResponse>
        where TQuery : IQuery<TResponse>
    {
        Task<QueryResult<TResponse>> Handle(TQuery query, RequestContext context);
    }
}
