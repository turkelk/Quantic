using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Quantic.Core.Util;

namespace Quantic.Cache.InMemory
{
    public static class MemoryCacheExtension
    {
        public static T GetOrAdd<T>(this IMemoryCache cache, string key, Func<T> factory)
        {
            return cache.GetOrCreate<T>(key, entry => new Lazy<T>(() =>
            {
                return factory.Invoke();
            }).Value);
        }

        public static Task<T> GetOrAddAsync<T>(this IMemoryCache cache,
            string key,
            Func<ICacheEntry, Task<T>> taskFactory)
        {
            return cache.GetOrCreateAsync<T>(key, async entry => await new AsyncLazy<T>(async () =>
            {
                return await taskFactory.Invoke(entry);
            }).Value);
        }

    }
}