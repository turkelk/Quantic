using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Quantic.Core;

namespace Quantic.FeatureManagement
{
    public class HandlerFeatureInfoProvider : IHandlerFeatureInfoProvider
    {
        private readonly IDictionary<string, HandlerFeatureInfo> handlerInfos = new Dictionary<string, HandlerFeatureInfo>();

        public HandlerFeatureInfoProvider(IEnumerable<Type> types)
        {
            IEnumerable<DecorateFeatureManagementAttribute> featureAttributes;

            foreach (var type in types)
            {
                featureAttributes = type.GetCustomAttributes<DecorateFeatureManagementAttribute>();
                if (featureAttributes != null && featureAttributes.Any())
                {
                    foreach (var @interface in type.GetInterfaces())
                    {
                        if (@interface.IsGenericType)
                        {
                            var typeDefinition = @interface.GetGenericTypeDefinition();

                            if (typeDefinition == typeof(IQueryHandler<,>)
                                || typeDefinition == typeof(ICommandHandler<>))
                            {
                                var requestName = @interface.GetGenericArguments().First().Name;
                                handlerInfos.TryAdd(requestName, new HandlerFeatureInfo(requestName, featureAttributes.Select(x => x.Feature).ToArray()));
                            }
                        }
                    }
                }
            }
        }

        public HandlerFeatureInfo GetHandlerInfo(string name)
        {
            return handlerInfos.ContainsKey(name) ? handlerInfos[name] : null;
        }        
        bool IsQueryHandler(Type givenType)
        {
            return givenType.GetInterfaces()
                .Any(x => x.IsGenericType
                    && x.GetGenericTypeDefinition() == typeof(IQueryHandler<,>));
        }

        bool IsCommandHandler(Type givenType)
        {
            return givenType.GetInterfaces()
                .Any(x => x.IsGenericType
                    && x.GetGenericTypeDefinition() == typeof(ICommandHandler<>));
        }
    }
}