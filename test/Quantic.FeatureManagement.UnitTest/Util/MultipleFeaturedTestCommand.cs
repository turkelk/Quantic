using System.Threading.Tasks;
using Quantic.Core;

namespace Quantic.FeatureManagement.UnitTest
{
    [DecorateFeatureManagement("FeatureQ")]
    [DecorateFeatureManagement("FeatureC")]    
    public class MultipleFeaturedTestCommandHandler : ICommandHandler<MultipleFeaturedTestCommand>
    {
        public Task<CommandResult> Handle(MultipleFeaturedTestCommand command, RequestContext context)
        {
            return Task.FromResult(CommandResult.Success);
        }
    }    
    public class MultipleFeaturedTestCommand : ICommand
    {
        
    }
}