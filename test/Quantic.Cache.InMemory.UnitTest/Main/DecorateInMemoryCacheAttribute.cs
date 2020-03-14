using System;
using Xunit;

namespace Quantic.Cache.InMemory.UnitTest
{
    public class DecorateInMemoryCacheAttributeTest
    {
        [Fact]
        public void ShouldSetDefaultValues()
        {
            var expectedExpiresInSecond = default(double);
            var expectedCacheKeyProviderType = default(Type);

            var attribute = new DecorateInMemoryCacheAttribute();

            Assert.Equal(expectedExpiresInSecond, attribute.ExpireInSeconds);
            Assert.Equal(expectedCacheKeyProviderType, attribute.CacheKeyProviderType);
        }  

        [Fact]
        public void ShouldSetExpireInSeconds()
        {
            var expectedExpiresInSecond = 1;
            var attribute = new DecorateInMemoryCacheAttribute(expireInSeconds:expectedExpiresInSecond);
            Assert.Equal(expectedExpiresInSecond, attribute.ExpireInSeconds);
        } 

        [Fact]
        public void ShouldSetCacheKeyProviderType()
        {
            var expectedCacheKeyProviderType = typeof(FooCacheKeyProvider);

            var attribute = new DecorateInMemoryCacheAttribute(cacheKeyProviderType:expectedCacheKeyProviderType);
            Assert.Equal(expectedCacheKeyProviderType, attribute.CacheKeyProviderType);
        } 
    }
}