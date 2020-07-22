using System;
using Xunit;

namespace Quantic.Cache.Redis.UnitTest
{
    public class DecorateRedisCacheAttributeTest
    {
        [Fact]
        public void ShouldSetDefaultValues()
        {
            var expectedExpiresInSecond = default(double);
            var expectedCacheKeyProviderType = default(Type);

            var attribute = new DecorateRedisCacheAttribute();

            Assert.Equal(expectedExpiresInSecond, attribute.ExpireInSeconds);
            Assert.Equal(expectedCacheKeyProviderType, attribute.CacheKeyProviderType);
        }  

        [Fact]
        public void ShouldSetExpireInSeconds()
        {
            var expectedExpiresInSecond = 1;
            var attribute = new DecorateRedisCacheAttribute(expireInSeconds:expectedExpiresInSecond);
            Assert.Equal(expectedExpiresInSecond, attribute.ExpireInSeconds);
        } 

        [Fact]
        public void ShouldSetCacheKeyProviderType()
        {
            var expectedCacheKeyProviderType = typeof(FooCacheKeyProvider);

            var attribute = new DecorateRedisCacheAttribute(cacheKeyProviderType:expectedCacheKeyProviderType);
            Assert.Equal(expectedCacheKeyProviderType, attribute.CacheKeyProviderType);
        } 
    }
}