using System;
using System.Threading.Tasks;
using Contracts;
using Quantic.Core;
using Quantic.MassTransit.RabbitMq;
using Quantic.Validation;

namespace Identity.Api.Commands
{
    [DecorateValidation]
    public class RegisterCommandHandler : ICommandHandler<RegisterCommand>
    {
        private readonly DataContext dataContext;
        private readonly IQuanticBus busControl;
        public RegisterCommandHandler(DataContext dataContext, IQuanticBus busControl)
        {
            this.busControl = busControl;
            this.dataContext = dataContext;

        }
        public async Task<CommandResult> Handle(RegisterCommand command, RequestContext context)
        {
            var user = new User
            {
                Guid = Guid.NewGuid(),
                Email = command.Email,
                Name = command.Name,
                LastName = command.LastName
            };

            dataContext.Users.Add(user);

            await dataContext.SaveChangesAsync();

            await busControl.Publish<UserRegistered>(new UserRegistered
            {
                Email = command.Email,
                Guid = user.Guid
            }, context);

            // send extension could be added and used instead of publish
            await busControl.Publish<SendMail>(new SendMail
            {
                To = command.Email,
                TemplateCode = "user_register_verify",   
                Params = { { 0,user.Guid } }             
            }, context);

            return new CommandResult(user.Guid);
        }
    }
}