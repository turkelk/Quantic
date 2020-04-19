using System.Linq;
using System.Threading.Tasks;
using Quantic.Core;

namespace Quantic.FeatureManagement
{
    public class FeatureManagementCommandHandlerDecorator<TCommand> : ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        private readonly FeatureSettingsHolder featureSettingsHolder;
        private readonly IHandlerFeatureInfoProvider handlerFeatureInfoProvider;
        private readonly ICommandHandler<TCommand> decoratedRequestHandler;

        public FeatureManagementCommandHandlerDecorator(ICommandHandler<TCommand> decoratedRequestHandler,
            FeatureSettingsHolder featureSettingsHolder,
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

            bool allFeaturesUsed = true;

            foreach (var feature in handlerInfo.Features)
            {
                if (context.GetHeaderValue($"{FeatureHeader.Prefix}-{feature}") == null)
                {
                    allFeaturesUsed = false;
                }
            }

            if (allFeaturesUsed)
                return await decoratedRequestHandler.Handle(command, context);

            bool allFeaturesEnabled = true;

            foreach (var feature in handlerInfo.Features ?? Enumerable.Empty<string>())
            {
                if (!featureSettingsHolder.FatureEnabled(feature, context))
                {
                    allFeaturesEnabled = false;
                }
            }

            if (!allFeaturesEnabled)
                return CommandResult.WithCode(FeatureMessages.FeatureNotEnabled);


            var usedFeatureHeaders = handlerInfo.Features.Select(feature => $"{FeatureHeader.Prefix}-{feature}");

            foreach (var header in usedFeatureHeaders)
            {
                context.Headers.Add(header, handlerInfo.Name);
            }

            return await decoratedRequestHandler.Handle(command, context);
        }
    }
}
