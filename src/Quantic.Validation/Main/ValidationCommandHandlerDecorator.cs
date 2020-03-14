using System.Linq;
using System.Threading.Tasks;
using Quantic.Core;

namespace Quantic.Validation
{
    public class ValidationCommandHandlerDecorator<TCommand> : ICommandHandler<TCommand>
       where TCommand : ICommand
    {
        private readonly ICommandHandler<TCommand> decoratedRequestHandler;
        private readonly QuanticValidator<TCommand> validator;

        public ValidationCommandHandlerDecorator(ICommandHandler<TCommand> decoratedRequestHandler,
            QuanticValidator<TCommand> validator)
        {
            this.decoratedRequestHandler = decoratedRequestHandler;
            this.validator = validator;
        }

        public async Task<CommandResult> Handle(TCommand command, RequestContext context)
        {
            validator.Context = context;
            
            var validationResult = await validator.ValidateAsync(command);

            return validationResult.IsValid
                ? await decoratedRequestHandler.Handle(command, context)
                : new CommandResult(validationResult
                    .Errors.Select(x=> new ValidationFailure(x.ErrorCode, x.ErrorMessage))
                    .ToArray());
        }
    }
}
