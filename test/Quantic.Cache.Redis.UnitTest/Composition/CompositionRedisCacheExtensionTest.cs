using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Quantic.Core;
using Quantic.Cache.Redis;
using Microsoft.Extensions.Caching.Distributed;

namespace Quantic.Cache.Redis.UnitTest
{
    public class CompositionRedisCacheExtensionTest
    {
        [Fact]
        public void ShouldRegisterRedisCache()
        {
            var builder = new ServiceCollection();

            builder.AddQuantic(opt =>
            {
                opt.Assemblies = new System.Reflection.Assembly[]
                {
                    typeof(TestCompositonQuery).Assembly
                };
            }).AddRedisCacheDecorator(opt=>
            {

            });

            var container = builder.BuildServiceProvider();

            var redisCache = container.GetService<IDistributedCache>();

            Assert.NotNull(redisCache);
        }


        [Fact]
        public void ShouldRegisterQueryInfoProvider()
        {
            var builder = new ServiceCollection();

            builder.AddQuantic(opt =>
            {
                opt.Assemblies = new System.Reflection.Assembly[]
                {
                    typeof(TestCompositonQuery).Assembly
                };
            }).AddRedisCacheDecorator(opt =>
            {

            });

            var container = builder.BuildServiceProvider();

            var queryInfoProvider = container.GetService<IQueryInfoProvider>();

            Assert.NotNull(queryInfoProvider);
        }

        [Fact]
        public void ShouldRegisterCacheDecorator()
        {
            var expectedType = typeof(RedisCacheQueryHandlerDecorator<,>).GetGenericTypeDefinition();

            var builder = new ServiceCollection();

            builder.AddQuantic(opt =>
            {
                opt.Assemblies = new System.Reflection.Assembly[]
                {
                    typeof(TestCompositonQueryHandler).Assembly
                };
            }).AddRedisCacheDecorator();

            var container = builder.BuildServiceProvider();

            var queryHandler = container.GetService<IQueryHandler<TestCompositonQuery, string>>();

            Assert.NotNull(queryHandler);

            Assert.Equal(expectedType, queryHandler.GetType().GetGenericTypeDefinition());
        }

        [Fact]
        public void ShouldNotRegisterQueryDecorator()
        {
            var expectedType = typeof(TestCompositonWithoutAttributeQueryHandler);

            var builder = new ServiceCollection();

            builder.AddQuantic(opt =>
            {
                opt.Assemblies = new System.Reflection.Assembly[]
                {
                    typeof(TestCompositonWithoutAttributeQuery).Assembly
                };
            }).AddRedisCacheDecorator();

            var container = builder.BuildServiceProvider();

            var queryHandler = container.GetService<IQueryHandler<TestCompositonWithoutAttributeQuery, string>>();

            Assert.NotNull(queryHandler);

            Assert.Equal(expectedType, queryHandler.GetType());
        }


        public class TestCompositonQuery : IQuery<string> { }

        [DecorateRedisCache]
        public class TestCompositonQueryHandler : IQueryHandler<TestCompositonQuery, string>
        {
            public Task<QueryResult<string>> Handle(TestCompositonQuery query, RequestContext context)
            {
                return Task.FromResult(new QueryResult<string>("ok"));
            }
        }

        public class TestCompositonWithoutAttributeQuery : IQuery<string> { }

        public class TestCompositonWithoutAttributeQueryHandler : IQueryHandler<TestCompositonWithoutAttributeQuery, string>
        {
            public Task<QueryResult<string>> Handle(TestCompositonWithoutAttributeQuery query, RequestContext context)
            {
                return Task.FromResult(new QueryResult<string>("ok"));
            }
        }
    }
}
