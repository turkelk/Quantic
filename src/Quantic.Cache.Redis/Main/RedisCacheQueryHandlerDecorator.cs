using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Quantic.Core;

namespace Quantic.Cache.Redis
{
    public class RedisCacheQueryHandlerDecorator<TQuery, TResponse> : IQueryHandler<TQuery, TResponse>
       where TQuery : IQuery<TResponse>
    {
        private readonly IQueryHandler<TQuery, TResponse> decoratedRequestHandler;
        private readonly IDistributedCache distributedCache;
        private readonly IQueryInfoProvider queryInfoProvider;

        public RedisCacheQueryHandlerDecorator(IQueryHandler<TQuery, TResponse> decoratedRequestHandler,
            IDistributedCache distributedCache,
            IQueryInfoProvider queryInfoProvider)
        {
            this.decoratedRequestHandler = decoratedRequestHandler;
            this.distributedCache = distributedCache;
            this.queryInfoProvider = queryInfoProvider;
        }

        public async Task<QueryResult<TResponse>> Handle(TQuery query, RequestContext context)
        {
            var queryInfo = queryInfoProvider.GetQueryInfo(query.GetType().Name);

            // no way but lets check. no need to break the call.
            if (queryInfo == null || !queryInfo.HasCache)
            {
                //  log critical ?
                return await decoratedRequestHandler.Handle(query, context);
            }

            string cacheEntryKey = "";

            cacheEntryKey = GetCacheEntryKey(query, context, queryInfo);

            var result = await distributedCache.GetOrAddAsync(cacheEntryKey, async entry =>
            {
                var getResult = await decoratedRequestHandler.Handle(query, context);

                if (getResult.HasError)
                {
                    throw new Exception($"Cache handler {((Result)getResult).ErrorsToString()}");
                }

                if (queryInfo.CacheOption.ExpireInSeconds > 0)
                {
                    entry.SetAbsoluteExpiration(new DateTimeOffset(DateTime.Now.AddSeconds(queryInfo.CacheOption.ExpireInSeconds)));
                }

                return getResult.Result;
            });

            return new QueryResult<TResponse>(result);
        }

        private static string GetCacheEntryKey(TQuery query, RequestContext context, QueryInfo queryInfo)
        {
            string cacheEntryKey;
            if (queryInfo.CacheOption.CacheKeyProviderType != default)
            {
                string key = null;

                try
                {
                    key = ((ICacheKeyProvider)Activator.CreateInstance(queryInfo.CacheOption.CacheKeyProviderType, query, context)).GetKey();
                }
                // could be various exceptions. 
                // instead of handling one by one lets give propper message and include exception details in message
                catch (Exception ex)
                {
                    throw new CacheKeyProviderTypeException($"Key provider should implement {nameof(ICacheKeyProvider)} and must have public constructor that accepts query (IQuery<T>) and context (RequestContext) as constructor parameter. Error detail: {ex} ");
                }

                cacheEntryKey = $"{queryInfo.Name}:{key}";
            }
            else
            {
                cacheEntryKey = queryInfo.Name;
            }

            return cacheEntryKey;
        }
    }
}
