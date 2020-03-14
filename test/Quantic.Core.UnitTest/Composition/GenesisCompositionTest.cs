using System;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.DependencyInjection;

namespace Quantic.Core.Test
{
    public class QuanticCompositionTest
    {
        [Fact]
        public void Should_throw_argument_exception_for_null_assembly()
        {
            bool exceptionThrown = false;
            string paramName = "Assemblies";

            try
            {
                var builder = new ServiceCollection();

                builder.AddQuantic(opt =>
                {
                    opt.Assemblies = null;
                    //opt.AddAssemblies(null);
                });
            }
            catch (ArgumentException ex)
            {
                exceptionThrown = true;
                Assert.Equal(paramName, ex.ParamName);
            }

            Assert.True(exceptionThrown);
        }

        [Fact]
        public void Should_success_register_commandHandlers()
        {
            var builder = new ServiceCollection();

            builder.AddQuantic(opt =>
            {
                opt.Assemblies = new System.Reflection.Assembly[]
                {
                    typeof(TestCompositonCommandHandler).Assembly
                };
            });

            var sp = builder.BuildServiceProvider();

            var commandHandler = sp.GetService<ICommandHandler<TestCompositonCommand>>();

            Assert.NotNull(commandHandler);

            Assert.Equal(typeof(TestCompositonCommandHandler).FullName, commandHandler.GetType().FullName);
        }

        [Fact]
        public void Should_success_register_queryHandlers()
        {
            var builder = new ServiceCollection();

            builder.AddQuantic(opt =>
            {
                opt.Assemblies = new System.Reflection.Assembly[]
                {
                    typeof(TestCompositonQueryHandler).Assembly
                };
            });

            var sp = builder.BuildServiceProvider();

            var queryHandler = sp.GetService<IQueryHandler<TestCompositonQuery, string>>();

            Assert.NotNull(queryHandler);

            Assert.Equal(typeof(TestCompositonQueryHandler).FullName, queryHandler.GetType().FullName);
        }


        public class TestCompositonCommand : ICommand { }

        public class TestCompositonCommandHandler : ICommandHandler<TestCompositonCommand>
        {
            public Task<CommandResult> Handle(TestCompositonCommand command, RequestContext context)
            {
                return Task.FromResult(CommandResult.Success);
            }
        }

        public class TestCompositonQuery : IQuery<string>
        {
            public string ValueToCheck;
        }

        public class TestCompositonQueryHandler : IQueryHandler<TestCompositonQuery, string>
        {
            public Task<QueryResult<string>> Handle(TestCompositonQuery query, RequestContext context)
            {
                return Task.FromResult(new QueryResult<string>(query.ValueToCheck));
            }
        }
    }
}
