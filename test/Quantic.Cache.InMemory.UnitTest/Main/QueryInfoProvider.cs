using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit;

namespace Quantic.Cache.InMemory.UnitTest
{
    public class QueryInfoProviderTest
    {
        [Fact]
        public void Success()
        {
            var provider = new QueryInfoProvider(new List<Type> 
            {
                typeof(FooQueryHandler),
                typeof(BarQueryHandler)
            });

            var fooQueryInfo = provider.GetQueryInfo(typeof(FooQuery).Name);
            
            var fooQueryCacheAttribute = typeof(FooQueryHandler).GetCustomAttribute<DecorateInMemoryCacheAttribute>();
            var fooCacheOption = new CacheOption(fooQueryCacheAttribute.ExpireInSeconds, fooQueryCacheAttribute.CacheKeyProviderType);

            Assert.NotNull(fooQueryInfo);
            Assert.Equal(typeof(FooQueryHandler),fooQueryInfo.HandlerType);
            Assert.Equal(typeof(FooQuery).Name,fooQueryInfo.Name);
            Assert.True(fooQueryInfo.HasCache);
            Assert.Equal(fooCacheOption, fooQueryInfo.CacheOption);

            var barQueryInfo = provider.GetQueryInfo(typeof(BarQuery).Name);

            Assert.NotNull(barQueryInfo);

            Assert.Equal(typeof(BarQueryHandler),barQueryInfo.HandlerType);
            Assert.Equal(typeof(BarQuery).Name,barQueryInfo.Name);
            Assert.False(barQueryInfo.HasCache);
            Assert.Null(barQueryInfo.CacheOption);            
        }
    }
}