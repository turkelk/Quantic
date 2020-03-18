using System.Threading.Tasks;
using Contracts;
using MassTransit;
using Quantic.Core;
using Quantic.Core.Util;
using Quantic.MassTransit.RabbitMq;

namespace Notification.Api
{
    public class UserRegisteredConsumer : IConsumer<UserRegistered>
    {
        private readonly ICommandHandler<SaveUserCommand> saveUserCommandHandler;

        public UserRegisteredConsumer(ICommandHandler<SaveUserCommand> saveUserCommandHandler)
        {
            saveUserCommandHandler.Guard(nameof(saveUserCommandHandler));
            
            this.saveUserCommandHandler = saveUserCommandHandler;
        }

        public async Task Consume(ConsumeContext<UserRegistered> context)
        {
            await saveUserCommandHandler.Handle(new SaveUserCommand 
            {
                 Guid = context.Message.Guid , 
                 Email = context.Message.Email 
            }, context.RequestContext());
        }
    }
}