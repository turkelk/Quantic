using System;

namespace Quantic.Cache.InMemory
{
    [AttributeUsage(System.AttributeTargets.Class)]
    public class DecorateInMemoryCacheAttribute : System.Attribute
    {
        public double ExpireInSeconds { get; }
        public Type CacheKeyProviderType { get; }

        public DecorateInMemoryCacheAttribute(double expireInSeconds = 0.0, Type cacheKeyProviderType = default)
        {
            this.CacheKeyProviderType = cacheKeyProviderType;
            this.ExpireInSeconds = expireInSeconds;
        }
    }
}