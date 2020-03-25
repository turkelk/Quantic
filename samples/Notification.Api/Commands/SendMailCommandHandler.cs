using System.Threading.Tasks;
using Quantic.Core;

namespace Notification.Api
{
    public class SendMailCommandHandler : ICommandHandler<SendMailCommand>
    {
        public Task<CommandResult> Handle(SendMailCommand command, RequestContext context)
        {
            // send mail
            return Task.FromResult(CommandResult.Success);
        }
    }
}