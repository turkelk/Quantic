using System;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.DependencyInjection;

namespace Quantic.Core.Test
{
    public class QuanticCompositionTest
    {
        [Fact]
        public void ShouldThrowArgumentExceptionForNullAssembly()
        {
            // Assert
            Assert.Throws<ArgumentException>(()=> 
            {
                new ServiceCollection().AddQuantic(opt =>
                {
                    opt.Assemblies = null;
                });
            });
        }

        [Fact]
        public void ShoulRegisterCommandHandlers()
        {
            // Arrange
            var builder = new ServiceCollection();
            builder.AddQuantic(opt =>
            {
                opt.Assemblies = new System.Reflection.Assembly[]
                {
                    typeof(TestCompositonCommandHandler).Assembly
                };
            });
            
            // Act
            var sp = builder.BuildServiceProvider();
            var commandHandler = sp.GetService<ICommandHandler<TestCompositonCommand>>();
            
            // Assert
            Assert.NotNull(commandHandler);
            Assert.Equal(typeof(TestCompositonCommandHandler).FullName, commandHandler.GetType().FullName);
        }

        [Fact]
        public void ShoulRegisterQueryHandlers()
        {
            // Arrange
            var builder = new ServiceCollection();
            builder.AddQuantic(opt =>
            {
                opt.Assemblies = new System.Reflection.Assembly[]
                {
                    typeof(TestCompositonQueryHandler).Assembly
                };
            });

            // Act
            var sp = builder.BuildServiceProvider();
            var queryHandler = sp.GetService<IQueryHandler<TestCompositonQuery, string>>();
            
            // Assert
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
