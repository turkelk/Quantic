using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Quantic.Core;
using Xunit;

namespace Quantic.Log.UnitTest
{
    public class QuanticCompositionLogExtensionTest
    {        
       
        [Fact]
        public void ShouldRegisterLogDecoratorForCommandHandlers()
        {
            // Arrange
            var expectedType = typeof(LogCommandHandlerDecorator<>).GetGenericTypeDefinition();
            var builder = new ServiceCollection();
            builder.AddLogging();
            builder.AddQuantic(opt =>
            {
                opt.Assemblies = new System.Reflection.Assembly[]
                {
                    typeof(TestCompositonCommandHandler).Assembly
                };
            }).AddLogDecorator();
            var container = builder.BuildServiceProvider();

            // Act
            var commandHandler = container.GetService<ICommandHandler<TestCompositonCommand>>();

            // Assert
            Assert.NotNull(commandHandler);
            Assert.Equal(expectedType, commandHandler.GetType().GetGenericTypeDefinition());
        }

        [Fact]
        public void ShouldRegisterCustomRequestLogger()
        {
            // Arrange

            var builder = new ServiceCollection();
            builder.AddQuantic(opt =>
            {
                opt.Assemblies = new System.Reflection.Assembly[]
                {
                    typeof(TestCompositonCommandHandler).Assembly
                };
            }).AddLogDecorator(opt=>
            {
                opt.AddRequestLogger<ReqLogger>();
            });

            var container = builder.BuildServiceProvider();

            // Act
            var logger = container.GetService<IRequestLogger>();

            // Assert
            Assert.True(logger.GetType() == typeof(ReqLogger));
        }

        [Fact]
        public void ShouldRegisterDefaultRequestLogger()
        {
            // Arrange
            var builder = new ServiceCollection();            
            builder.AddLogging();
            builder.AddQuantic(opt =>
            {
                opt.Assemblies = new System.Reflection.Assembly[]
                {
                    typeof(TestCompositonCommandHandler).Assembly
                };
            }).AddLogDecorator();

            var container = builder.BuildServiceProvider();

            // Act
            var logger = container.GetService<IRequestLogger>();

            // Assert
            Assert.NotNull(logger);
        }

        [Fact]
        public void ShouldRegisterLogDecoratorForQueryHandlers()
        {
            // Arrange
            var expectedType = typeof(LogQueryHandlerDecorator<,>).GetGenericTypeDefinition();
            var builder = new ServiceCollection();
            
            builder.AddLogging();
            builder.AddQuantic(opt =>
            {
                opt.Assemblies = new System.Reflection.Assembly[]
                {
                    typeof(TestCompositonQueryHandler).Assembly
                };
            }).AddLogDecorator();
            builder.AddOptions();

            var container = builder.BuildServiceProvider();
            // Act
            var queryHandler = container.GetService<IQueryHandler<TestCompositonQuery,string>>();

            // Assert
            Assert.NotNull(queryHandler);
            Assert.Equal(expectedType, queryHandler.GetType().GetGenericTypeDefinition());
        }

        public class TestCompositonCommand : ICommand
        {
            public int CallCount;
        }

        public class TestCompositonCommandHandler : ICommandHandler<TestCompositonCommand>
        {
            public Task<CommandResult> Handle(TestCompositonCommand command, RequestContext context)
            {
                command.CallCount++;

                return Task.FromResult(CommandResult.Success);
            }
        }

        public class TestCompositonQuery : IQuery<string>
        {
            public int CallCount;
        }

        public class TestCompositonQueryHandler : IQueryHandler<TestCompositonQuery,string>
        {
            public Task<QueryResult<string>> Handle(TestCompositonQuery query, RequestContext context)
            {
                query.CallCount++;
                return Task.FromResult(new QueryResult<string>("OK"));
            }
        }

        public class ReqLogger : IRequestLogger
        {
            public Task Log(RequestLog log)
            {
                return Task.CompletedTask;
                                 
            }
        }
    }
}