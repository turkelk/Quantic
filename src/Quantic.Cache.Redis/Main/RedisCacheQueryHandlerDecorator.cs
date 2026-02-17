using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Quantic.Core;

namespace Quantic.Cache.Redis
{
    public class RedisCacheQueryHandlerDecorator<TQuery, TResponse> : IQueryHandler<TQuery, TResponse>
       where TQuery : IQuery<TResponse>
    {
        private readonly IQueryHandler<TQuery, TResponse> decoratedRequestHandler;
        private readonly IDistributedCache distributedCache;
        private readonly IQueryInfoProvider queryInfoProvider;
        private readonly ILogger logger;

        public RedisCacheQueryHandlerDecorator(IQueryHandler<TQuery, TResponse> decoratedRequestHandler,
            IDistributedCache distributedCache,
            IQueryInfoProvider queryInfoProvider,
            ILoggerFactory loggerFactory)
        {
            this.decoratedRequestHandler = decoratedRequestHandler;
            this.distributedCache = distributedCache;
            this.queryInfoProvider = queryInfoProvider;
            this.logger = loggerFactory.CreateLogger("RedisCacheQueryHandlerDecorator");
        }

        public async Task<QueryResult<TResponse>> Handle(TQuery query, RequestContext context)
        {
            logger.LogDebug("Cache decorator starting");

            var queryInfo = queryInfoProvider.GetQueryInfo(query.GetType().Name);

            logger.LogDebug($"Query info query name is  {queryInfo?.Name}");

            // no way but lets check. no need to break the call.
            if (queryInfo == null || !queryInfo.HasCache)
            {
                logger.LogDebug($"Query info is null or query info has cache is {queryInfo?.HasCache}");

                return await decoratedRequestHandler.Handle(query, context);
            }

            string cacheEntryKey = "";

            logger.LogDebug($"Getting CacheEntryKey");

            cacheEntryKey = GetCacheEntryKey(query, context, queryInfo);

            logger.LogDebug($"CacheEntryKey getted successfully: {cacheEntryKey}");

            QueryResult<TResponse> queryResult = default;

            var result = await distributedCache.GetOrAddAsync<TResponse>(cacheEntryKey, async entry =>
            {
                queryResult = await decoratedRequestHandler.Handle(query, context);

                if (queryResult.HasError)
                {
                    throw new Exception($"Cache handler error.Error:{queryResult.ErrorsToString()}");
                }

                if (queryInfo.CacheOption.ExpireInSeconds > 0)
                {
                    entry.SetAbsoluteExpiration(new DateTimeOffset(DateTime.Now.AddSeconds(queryInfo.CacheOption.ExpireInSeconds)));
                }

                return queryResult.Result;
            });

            if (queryResult?.Code == Messages.NotFound)
            {
                await distributedCache.RemoveAsync(cacheEntryKey);
                return queryResult;
            }

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
                    key = ((ICacheKeyProvider)Activator.CreateInstance(queryInfo.CacheOption.CacheKeyProviderType)).GetKey(query, context);
                }
                // could be various exceptions.
                // instead of handling one by one lets give propper message and include exception details in message
                catch (Exception ex)
                {
                    throw new CacheKeyProviderTypeException($"Key provider should implement {nameof(ICacheKeyProvider)} and must have public parameterless constructor. Error detail: {ex.ToString()} ");
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
