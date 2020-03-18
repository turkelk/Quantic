using Quantic.Core;

namespace Identity.Api
{
    public class GetUserByEmailQuery : IQuery<User> 
    {
        public string Email { get; set; }
    }
}