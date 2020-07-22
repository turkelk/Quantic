using System;
using Xunit;

namespace Quantic.Cache.Redis.UnitTest
{
    public class CacheOptionTest
    {
        [Fact]
        public void ShouldSetDefaultValues()
        {
            var expectedExpiresInSecond = default(double);
            var expectedCacheKeyProviderType = default(Type);

            var option = new CacheOption();

            Assert.Equal(expectedExpiresInSecond, option.ExpireInSeconds);
            Assert.Equal(expectedCacheKeyProviderType, option.CacheKeyProviderType);
        }  

        [Fact]
        public void ShouldSetExpireInSeconds()
        {
            var expectedExpiresInSecond = 1;
            var option = new CacheOption(expireInSeconds:expectedExpiresInSecond);
            Assert.Equal(expectedExpiresInSecond, option.ExpireInSeconds);
        } 

        [Fact]
        public void ShouldSetCacheKeyProviderType()
        {
            var expectedCacheKeyProviderType = typeof(FooCacheKeyProvider);

            var option = new CacheOption(cacheKeyProviderType:expectedCacheKeyProviderType);
            Assert.Equal(expectedCacheKeyProviderType, option.CacheKeyProviderType);
        }  

        [Fact]
        public void EqualTest()
        {
            object nullObject = null;
            var cacheKeyProvider = typeof(FooCacheKeyProvider);

            var option1 = new CacheOption(1, cacheKeyProvider);
            var option2 = new CacheOption(2, cacheKeyProvider);
            var option1_2 = new CacheOption(1, cacheKeyProvider);

            Assert.False(option1.Equals(option2));
            Assert.False(option1.Equals(null));
            Assert.False(option1.Equals(nullObject));

            Assert.True(option1.Equals(option1_2));
        }                                 
    }
}