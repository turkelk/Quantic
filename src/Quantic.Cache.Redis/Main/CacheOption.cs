using System;
using System.Diagnostics.CodeAnalysis;

namespace Quantic.Cache.Redis
{
    public class CacheOption : IEquatable<CacheOption>
    {   
        public double ExpireInSeconds { get; }
        public Type CacheKeyProviderType { get; }

        public CacheOption(double expireInSeconds = default, Type cacheKeyProviderType = default)
        {
            this.CacheKeyProviderType = cacheKeyProviderType;
            this.ExpireInSeconds = expireInSeconds;
        }

        public override bool Equals([AllowNull] object other)
        {
            if (other == null)
                return false;
                
            return Equals(other as CacheOption);
        }

        public bool Equals(CacheOption other)
        {
            if (other == null)
                return false;

            return this.ExpireInSeconds.Equals(other.ExpireInSeconds) &&  this.CacheKeyProviderType.Equals(CacheKeyProviderType);
        }     

        public override int GetHashCode() 
        {
            return $"{CacheKeyProviderType.FullName}_{ExpireInSeconds}".GetHashCode();            
        }        
    }
}