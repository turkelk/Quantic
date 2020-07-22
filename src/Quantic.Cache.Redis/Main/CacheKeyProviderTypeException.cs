using System;

namespace Quantic.Cache.Redis
{
    public class CacheKeyProviderTypeException : Exception
    {
        public CacheKeyProviderTypeException(string message) : base(message) { }
    }
}