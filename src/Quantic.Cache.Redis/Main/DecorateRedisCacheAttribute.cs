using System;

namespace Quantic.Cache.Redis
{
    [AttributeUsage(System.AttributeTargets.Class)]
    public class DecorateRedisCacheAttribute : System.Attribute
    {
        public double ExpireInSeconds { get; }
        public Type CacheKeyProviderType { get; }

        public DecorateRedisCacheAttribute(double expireInSeconds = 0.0, Type cacheKeyProviderType = default)
        {
            this.CacheKeyProviderType = cacheKeyProviderType;
            this.ExpireInSeconds = expireInSeconds;
        }
    }
}