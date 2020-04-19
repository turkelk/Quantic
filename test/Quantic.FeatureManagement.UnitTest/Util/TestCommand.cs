using System.Threading.Tasks;
using Quantic.Core;

namespace Quantic.FeatureManagement.UnitTest
{
    [DecorateFeatureManagement("FeatureC")]
    public class TestCommandHandler : ICommandHandler<TestCommand>
    {
        public Task<CommandResult> Handle(TestCommand command, RequestContext context)
        {
            return Task.FromResult(CommandResult.Success);
        }
    }
    public class TestCommand : ICommand { }
}