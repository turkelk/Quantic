using Quantic.Core;

namespace Identity.Api
{
    public class RegisterCommand : ICommand
    {
        public string Name {get;set;}
        public string LastName {get;set;}
        public string Password { get; set; }
        public string Email { get; set; }
    }
}