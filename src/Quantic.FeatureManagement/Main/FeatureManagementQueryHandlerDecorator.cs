using System;
using System.Linq;
using System.Threading.Tasks;
using Quantic.Core;

namespace Quantic.FeatureManagement
{
    public class FeatureManagementQueryHandlerDecorator<TQuery, TResponse> : IQueryHandler<TQuery, TResponse>
               where TQuery : IQuery<TResponse>
    {
        private readonly FeatureSettingsHolder featureSettingsHolder;
        private readonly IHandlerFeatureInfoProvider handlerFeatureInfoProvider;
        private readonly IQueryHandler<TQuery,TResponse> decoratedRequestHandler;

        public FeatureManagementQueryHandlerDecorator(IQueryHandler<TQuery,TResponse> decoratedRequestHandler,
            FeatureSettingsHolder featureSettingsHolder,
            IHandlerFeatureInfoProvider handlerFeatureInfoProvider)
        {
            this.decoratedRequestHandler = decoratedRequestHandler;
            this.featureSettingsHolder = featureSettingsHolder;
            this.handlerFeatureInfoProvider = handlerFeatureInfoProvider;
        }

        public async Task<QueryResult<TResponse>> Handle(TQuery query, RequestContext context)
        {
            var handlerInfo = handlerFeatureInfoProvider.GetHandlerInfo(query.GetType().Name);

            if (handlerInfo == null)
                return await decoratedRequestHandler.Handle(query, context);

            bool allFeaturesUsed = true;

            foreach (var feature in handlerInfo.Features)
            {
                if (context.GetHeaderValue($"{FeatureHeader.Prefix}-{feature}") == null)
                {
                    allFeaturesUsed = false;
                }
            }

            if (allFeaturesUsed)
                return await decoratedRequestHandler.Handle(query, context);

            bool allFeaturesEnabled = true;

            foreach (var feature in handlerInfo.Features)
            {
                if (!featureSettingsHolder.FatureEnabled(feature, context))
                {
                    allFeaturesEnabled = false;
                }
            }

            if (!allFeaturesEnabled)
                return QueryResult<TResponse>.WithCode(default(TResponse), FeatureMessages.FeatureNotEnabled);

            var usedFeatureHeaders = handlerInfo.Features.Select(x => $"{FeatureHeader.Prefix}-{x}");

            foreach (var header in usedFeatureHeaders)
            {
                context.Headers.Add(header, handlerInfo.Name);
            }
            
            return await decoratedRequestHandler.Handle(query, context);            
        }            
    }
}
