using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Redis;
using Moq;
using Quantic.Core;
using Xunit;

namespace Quantic.Cache.Redis.UnitTest
{
    public class RedisCacheQueryHandlerDecorator
    {
        private readonly IDistributedCache cache;
        private readonly IQueryInfoProvider queryInfoProvider;
        public RedisCacheQueryHandlerDecorator()
        {
            
            cache = new RedisCache(new RedisCacheOptions());
            
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
            var mockHandler = new Mock<IQueryHandler<NonRegisteredQuery,int>>();
            var query = new NonRegisteredQuery();
            var cacheQueryHandlerDecorator =  new RedisCacheQueryHandlerDecorator<NonRegisteredQuery, int>(mockHandler.Object, cache, queryInfoProvider);            
            
            mockHandler
            .Setup(x=>x.Handle(query, Util.Context))
            .ReturnsAsync(new QueryResult<int>(1));
            
            // Act
            var result = await cacheQueryHandlerDecorator.Handle(query, Util.Context);

            // Assert
            Assert.True(result.IsSuccess);  
            mockHandler.Verify(x=>x.Handle(query, Util.Context), Times.Once);

            // Act         
            result = await cacheQueryHandlerDecorator.Handle(query, Util.Context);

            // Assert
            Assert.True(result.IsSuccess);  
            mockHandler.Verify(x=>x.Handle(query, Util.Context), Times.Exactly(2));           
        }  

        [Fact]
        public async Task ShouldThrowCacheKeyProviderTypeException()
        {
            // Arrange
            var qip = new QueryInfoProvider(new List<Type> 
            {
                typeof(WrongCacheKeyProviderQueryHandler)
            });
            var cacheQueryHandlerDecorator =  new RedisCacheQueryHandlerDecorator<WrongCacheKeyProviderQuery, string>(new WrongCacheKeyProviderQueryHandler(), cache, qip);    
            var query = new WrongCacheKeyProviderQuery();            
            bool exceptionThrown = false;

            // Act
            try{
                 var result = await cacheQueryHandlerDecorator.Handle(query, Util.Context);
            }catch (CacheKeyProviderTypeException) {
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

            var cacheQueryHandlerDecorator =  new RedisCacheQueryHandlerDecorator<CacheKeyProviderDoesNotHavePropperConstructorQuery, string>(new CacheKeyProviderDoesNotHavePropperConstructorQueryHandler(), cache, qip);            
            var query = new CacheKeyProviderDoesNotHavePropperConstructorQuery();            
            bool exceptionThrown = false;

            // Act            
            try{
                 var result = await cacheQueryHandlerDecorator.Handle(query, Util.Context);
            }catch (CacheKeyProviderTypeException) {
                exceptionThrown = true;
            }
           
           // Assert
           Assert.True(exceptionThrown);
        }                      
 

        [Fact]
        public async Task ShouldCacheWithKey()
        {
            // Arrange
            
            string key = "key";
            string value = "value";
            string newKey = "newKey";
            string newValue = "newValue";
            var fooQuery = new FooQuery { ValueToCache = value, CacheKey = key };

            var mock = new Mock<IDistributedCache>();
            var sequence = new MockSequence();

            mock.InSequence(sequence).Setup(x => x.GetAsync($"FooQuery:{key}",default))
                .ReturnsAsync((byte[])null);
            mock.InSequence(sequence).Setup(x => x.GetAsync($"FooQuery:{key}", default))
                .ReturnsAsync(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(value)));
            mock.InSequence(sequence).Setup(x => x.GetAsync($"FooQuery:{newKey}", default))
                .ReturnsAsync(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(newValue)));
            var cache = mock.Object;
                     
            var cacheQueryHandlerDecorator =  new RedisCacheQueryHandlerDecorator<FooQuery, string>(new FooQueryHandler(), cache, queryInfoProvider);  
            

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