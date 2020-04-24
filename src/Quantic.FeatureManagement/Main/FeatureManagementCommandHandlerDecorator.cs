using System.Linq;
using System.Threading.Tasks;
using Quantic.Core;

namespace Quantic.FeatureManagement
{
    public class FeatureManagementCommandHandlerDecorator<TCommand> : ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        private readonly SettingsHolder featureSettingsHolder;
        private readonly IHandlerFeatureInfoProvider handlerFeatureInfoProvider;
        private readonly ICommandHandler<TCommand> decoratedRequestHandler;

        public FeatureManagementCommandHandlerDecorator(ICommandHandler<TCommand> decoratedRequestHandler,
            SettingsHolder featureSettingsHolder,
            IHandlerFeatureInfoProvider handlerFeatureInfoProvider)
        {
            this.decoratedRequestHandler = decoratedRequestHandler;
            this.featureSettingsHolder = featureSettingsHolder;
            this.handlerFeatureInfoProvider = handlerFeatureInfoProvider;
        }

        public async Task<CommandResult> Handle(TCommand command, RequestContext context)
        {
            var handlerInfo = handlerFeatureInfoProvider.GetHandlerInfo(command.GetType().Name);

            if (handlerInfo == null)
                return await decoratedRequestHandler.Handle(command, context);
                
            var allFeaturesEnabled = handlerInfo.Features.All(feature => 
            {
                return featureSettingsHolder.Settings.FirstOrDefault(x=>x.FeatureName == feature).Enabled(context);
            });

            if (!allFeaturesEnabled)
                return CommandResult.WithCode(Msg.FeatureNotEnabled);

            var usedFeatureHeaders = handlerInfo.Features.Select(featureName => $"{Header.Prefix}-{featureName}");

            foreach (var header in usedFeatureHeaders)
            {
                context.Add(header, handlerInfo.Name);
            }

            return await decoratedRequestHandler.Handle(command, context);
        }
    }
}
