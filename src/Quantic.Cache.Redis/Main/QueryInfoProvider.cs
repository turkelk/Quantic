using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Quantic.Core;

namespace Quantic.Cache.Redis
{
    public class QueryInfoProvider : IQueryInfoProvider
    {
        private readonly IDictionary<string, QueryInfo> queryInfos = new Dictionary<string, QueryInfo>();

        public QueryInfoProvider(IEnumerable<Type> types)
        {
            DecorateRedisCacheAttribute useCacheAttribute;
            string querName;

            foreach (var type in types)
            {
                if (IsQueryHandler(type))
                {
                    useCacheAttribute = type.GetCustomAttribute<DecorateRedisCacheAttribute>();

                    querName = type.GetInterfaces()
                     .Single(x => x.IsGenericType
                         && x.GetGenericTypeDefinition() == typeof(IQueryHandler<,>))
                     .GetGenericArguments()
                     .First()
                     .Name;

                    queryInfos.TryAdd(querName, new QueryInfo
                    {
                        Name = querName,
                        HandlerType = type,
                        HasCache = useCacheAttribute != null,
                        CacheOption = useCacheAttribute != null
                            ? new CacheOption(useCacheAttribute.ExpireInSeconds, useCacheAttribute.CacheKeyProviderType)
                            : null
                    });
                }
            }

            static bool IsQueryHandler(Type givenType)
            {
                return givenType.GetInterfaces()
                    .Any(x => x.IsGenericType
                        && x.GetGenericTypeDefinition() == typeof(IQueryHandler<,>));
            }
        }

        public QueryInfo GetQueryInfo(string fullName)
        {
            return queryInfos.ContainsKey(fullName) ? queryInfos[fullName] : null;
        }
    }


    public class QueryInfo
    {
        public string Name { get; set; }
        public bool HasCache { get; set; }
        public Type HandlerType { get; set; }
        public CacheOption CacheOption { get; set; }
    }
}