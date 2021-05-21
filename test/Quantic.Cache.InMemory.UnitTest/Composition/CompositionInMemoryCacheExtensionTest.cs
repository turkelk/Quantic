using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Quantic.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Quantic.Cache.InMemory.UnitTest
{
    public class CompositionInMemoryCacheExtensionTest
    {
        [Fact]
        public void ShouldRegisterInMemoryCache()
        {
            var builder = new ServiceCollection();

            builder.AddQuantic(opt =>
            {
                opt.Assemblies = new System.Reflection.Assembly[]
                {
                    typeof(TestCompositonQuery).Assembly
                };
            }).AddMemoryCacheDecorator(opt =>
            {

            });

            var container = builder.BuildServiceProvider();

            var memoryCache = container.GetService<IMemoryCache>();

            Assert.NotNull(memoryCache);
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
            }).AddMemoryCacheDecorator(opt =>
            {

            });

            var container = builder.BuildServiceProvider();

            var queryInfoProvider = container.GetService<IQueryInfoProvider>();

            Assert.NotNull(queryInfoProvider);
        }

        [Fact]
        public void ShouldRegisterCacheDecorator()
        {
            var expectedType = typeof(InMemoryCacheQueryHandlerDecorator<,>).GetGenericTypeDefinition();

            var builder = new ServiceCollection();
            builder.AddTransient<ILoggerFactory, NullLoggerFactory>();

            builder.AddQuantic(opt =>
            {
                opt.Assemblies = new System.Reflection.Assembly[]
                {
                    typeof(TestCompositonQueryHandler).Assembly
                };
            }).AddMemoryCacheDecorator();

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
            }).AddMemoryCacheDecorator();

            var container = builder.BuildServiceProvider();

            var queryHandler = container.GetService<IQueryHandler<TestCompositonWithoutAttributeQuery, string>>();

            Assert.NotNull(queryHandler);

            Assert.Equal(expectedType, queryHandler.GetType());
        }


        public class TestCompositonQuery : IQuery<string> { }

        [DecorateInMemoryCache]
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
