using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace Quantic.Cache.Redis
{
    public static class RedisCacheExtension
    {
        public static T GetOrAdd<T>(this IDistributedCache cache, string key, Func<T> factory)
        {
            string jsonValue = cache.GetString(key);
            if(jsonValue != null)
                return JsonSerializer.Deserialize<T>(jsonValue);


            T value = factory.Invoke();
            cache.SetString(key, JsonSerializer.Serialize(value));
            return value;
        }

        public static async Task<T> GetOrAddAsync<T>(this IDistributedCache cache,
            string key,
            Func<DistributedCacheEntryOptions, Task<T>> taskFactory)
        {
            string jsonValue = await cache.GetStringAsync(key);
            if (jsonValue != null)
                return JsonSerializer.Deserialize<T>(jsonValue);

            var cacheEntryOptions = new DistributedCacheEntryOptions();
            T value = await taskFactory.Invoke(cacheEntryOptions);
            await cache.SetStringAsync(key, JsonSerializer.Serialize(value), cacheEntryOptions);
            return value;
        }

    }
}