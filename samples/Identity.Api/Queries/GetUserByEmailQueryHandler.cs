using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Quantic.Core;

namespace Identity.Api
{
    public class GetUserByEmailQueryHandler : IQueryHandler<GetUserByEmailQuery, User>
    {
        private readonly DataContext dataContext;

        public GetUserByEmailQueryHandler(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public async Task<QueryResult<User>> Handle(GetUserByEmailQuery query, RequestContext context)
        {
            var user = await dataContext.Users.FirstOrDefaultAsync(u => u.Email == query.Email);

            return user == null 
                ? new QueryResult<User>(null, Msg.UserNotExistByMail) 
                : new QueryResult<User>(user);
        }
    }
}