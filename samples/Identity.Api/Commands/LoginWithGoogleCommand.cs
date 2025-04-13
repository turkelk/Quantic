using Quantic.Core;

namespace Identity.Api.Commands
{
    public class LoginWithGoogleCommand : ICommand
    {
        public string IdToken { get; set; }
        public string AccessToken { get; set; }
    }
} 