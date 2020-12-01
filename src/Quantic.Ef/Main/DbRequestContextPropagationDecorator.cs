using System.Threading.Tasks;
using Quantic.Core;
using Quantic.Ef.Composition;
using System;

namespace Quantic.Ef.Main
{
    public class DbRequestContextPropagationDecorator<TCommand> : ICommandHandler<TCommand>
       where TCommand : ICommand
    {
        private readonly ICommandHandler<TCommand> decoratedRequestHandler;
        private readonly QuanticEfConfig efConfig;
        private readonly IServiceProvider serviceProvider;

        public DbRequestContextPropagationDecorator(ICommandHandler<TCommand> decoratedRequestHandler,
            QuanticEfConfig efConfig,
            IServiceProvider serviceProvider)
        {
            this.decoratedRequestHandler = decoratedRequestHandler;
            this.efConfig = efConfig;
            this.serviceProvider = serviceProvider;
        }

        public async Task<CommandResult> Handle(TCommand command, RequestContext context)
        {
            var dbContext = serviceProvider.GetService(efConfig.DbContextType);
            ((QuanticDbContext)dbContext).RequestContext = context;
            return await decoratedRequestHandler.Handle(command, context);
        }
    }
}