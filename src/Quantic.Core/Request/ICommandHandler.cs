using System.Threading.Tasks;

namespace Quantic.Core
{
    public interface ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        Task<CommandResult> Handle(TCommand command, RequestContext context);
    }
}
