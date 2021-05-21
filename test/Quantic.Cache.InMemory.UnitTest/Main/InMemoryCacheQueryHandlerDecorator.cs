using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Quantic.Core;
using Xunit;

namespace Quantic.Cache.InMemory.UnitTest
{
    public class InMemoryCacheQueryHandlerDecorator
    {
        private readonly IMemoryCache cache;
        private readonly IQueryInfoProvider queryInfoProvider;

        public InMemoryCacheQueryHandlerDecorator()
        {
            cache = new MemoryCache(new MemoryCacheOptions());

            queryInfoProvider = new QueryInfoProvider(new List<Type>
            {
                typeof(FooQueryHandler),
                typeof(BarQueryHandler)
            });
        }

        [Fact]
        public async Task ShouldCallHandlerWithoutCacheIfHandlerIsNotRegisteredToProvider()
        {
            // Arrange
            var mockHandler = new Mock<IQueryHandler<NonRegisteredQuery, int>>();
            var query = new NonRegisteredQuery();

            var cacheQueryHandlerDecorator = new InMemoryCacheQueryHandlerDecorator<NonRegisteredQuery, int>(mockHandler.Object, cache, queryInfoProvider, new NullLoggerFactory());

            mockHandler
            .Setup(x => x.Handle(query, Util.Context))
            .ReturnsAsync(new QueryResult<int>(1));

            // Act
            var result = await cacheQueryHandlerDecorator.Handle(query, Util.Context);

            // Assert
            Assert.True(result.IsSuccess);
            mockHandler.Verify(x => x.Handle(query, Util.Context), Times.Once);

            // Act         
            result = await cacheQueryHandlerDecorator.Handle(query, Util.Context);

            // Assert
            Assert.True(result.IsSuccess);
            mockHandler.Verify(x => x.Handle(query, Util.Context), Times.Exactly(2));
        }

        [Fact]
        public async Task ShouldThrowCacheKeyProviderTypeException()
        {
            // Arrange
            var qip = new QueryInfoProvider(new List<Type>
            {
                typeof(WrongCacheKeyProviderQueryHandler)
            });
            var cacheQueryHandlerDecorator = new InMemoryCacheQueryHandlerDecorator<WrongCacheKeyProviderQuery, string>(new WrongCacheKeyProviderQueryHandler(), cache, qip, new NullLoggerFactory());
            var query = new WrongCacheKeyProviderQuery();
            bool exceptionThrown = false;

            // Act
            try
            {
                var result = await cacheQueryHandlerDecorator.Handle(query, Util.Context);
            }
            catch (CacheKeyProviderTypeException)
            {
                exceptionThrown = true;
            }

            // Assert
            Assert.True(exceptionThrown);
        }

        [Fact]
        public async Task ShouldThrowCacheKeyProviderTypeExceptionIfCacheKeyProviderTypeDoesNotHavePropperConstructor()
        {
            // Arrange
            var qip = new QueryInfoProvider(new List<Type>
            {
                typeof(CacheKeyProviderDoesNotHavePropperConstructorQueryHandler)
            });

            var cacheQueryHandlerDecorator = new InMemoryCacheQueryHandlerDecorator<CacheKeyProviderDoesNotHavePropperConstructorQuery, string>(new CacheKeyProviderDoesNotHavePropperConstructorQueryHandler(), cache, qip, new NullLoggerFactory());
            var query = new CacheKeyProviderDoesNotHavePropperConstructorQuery();
            bool exceptionThrown = false;

            // Act            
            try
            {
                var result = await cacheQueryHandlerDecorator.Handle(query, Util.Context);
            }
            catch (CacheKeyProviderTypeException)
            {
                exceptionThrown = true;
            }

            // Assert
            Assert.True(exceptionThrown);
        }

        [Fact]
        public async Task ShouldCacheExpireInSecondsDuration()
        {
            // Arrange
            var cacheQueryHandlerDecorator = new InMemoryCacheQueryHandlerDecorator<FooQuery, string>(new FooQueryHandler(), cache, queryInfoProvider, new NullLoggerFactory());

            string oldValue = "foo";
            string newValue = "fooNew";

            var fooQuery = new FooQuery { ValueToCache = oldValue, CacheKey = "key" };

            var result = await cacheQueryHandlerDecorator
                .Handle(fooQuery, Util.Context);

            // Assert
            Assert.Equal(result.Result, oldValue);

            // Arrange  
            fooQuery.ValueToCache = newValue;

            // Act
            result = await cacheQueryHandlerDecorator
                .Handle(fooQuery, Util.Context);

            // Assert
            Assert.Equal(result.Result, oldValue);

            // Arrange 
            Thread.Sleep(new TimeSpan(0, 0, 3));

            // Act
            result = await cacheQueryHandlerDecorator
                .Handle(fooQuery, Util.Context);

            // Assert
            Assert.Equal(result.Result, newValue);
        }

        [Fact]
        public async Task ShouldCacheWithKey()
        {
            // Arrange
            string key = "key";
            string value = "value";
            string newKey = "newKey";
            string newValue = "newValue";
            var cacheQueryHandlerDecorator = new InMemoryCacheQueryHandlerDecorator<FooQuery, string>(new FooQueryHandler(), cache, queryInfoProvider, new NullLoggerFactory());
            var fooQuery = new FooQuery { ValueToCache = value, CacheKey = key };

            // Act
            var result = await cacheQueryHandlerDecorator
                .Handle(fooQuery, Util.Context);

            // Assert
            Assert.Equal(result.Result, value);

            // Arrange
            // result should be old value for same key                        
            fooQuery.ValueToCache = newValue;

            // Act
            result = await cacheQueryHandlerDecorator
                .Handle(fooQuery, Util.Context);

            // Assert
            Assert.Equal(result.Result, value);

            // result should be new value for new key  

            // Arrange
            fooQuery.ValueToCache = newValue;
            fooQuery.CacheKey = newKey;

            // Act
            result = await cacheQueryHandlerDecorator
                .Handle(fooQuery, Util.Context);

            // Assert
            Assert.Equal(result.Result, newValue);

        }
    }
}