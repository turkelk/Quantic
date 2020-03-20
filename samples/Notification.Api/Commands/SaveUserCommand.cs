using System;
using System.Threading.Tasks;
using Quantic.Core;
using Quantic.Core.Util;
using Quantic.Validation;

namespace Notification.Api
{
    [DecorateValidation]
    public class SaveUserCommandHandler : ICommandHandler<SaveUserCommand>
    {
        private readonly DataContext dataContext;

        public SaveUserCommandHandler(DataContext dataContext)
        {
            dataContext.Guard(nameof(dataContext));
            
            this.dataContext = dataContext;
        }

        public async Task<CommandResult> Handle(SaveUserCommand command, RequestContext context)
        {
            await dataContext.Users.AddAsync(new User
            {
                Guid = command.Guid, 
                Email = command.Email
            });

            await dataContext.SaveChangesAsync();

            return CommandResult.Success;
        }
    }

    public class SaveUserCommand : ICommand
    {
        public string Email {get;set;}
        public Guid Guid {get;set;} 
    }
}