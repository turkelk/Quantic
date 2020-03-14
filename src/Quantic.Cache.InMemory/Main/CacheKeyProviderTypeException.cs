using System;

namespace Quantic.Cache.InMemory
{
    public class CacheKeyProviderTypeException : Exception
    {
        public CacheKeyProviderTypeException(string message) : base(message) { }
    }
}