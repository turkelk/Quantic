using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Quantic.Core;
using Xunit;

namespace Quantic.Validation.UnitTest
{
    public class CompositionValidationExtensionTest
    {
        [Fact]
        public void ShouldRegisterValidationDecoratorForCommandHandlersInAssemblyOfGenesisBuilder()
        {
            // Arrange
            var expectedType = typeof(ValidationCommandHandlerDecorator<>).GetGenericTypeDefinition();

            var builder = new ServiceCollection();

            builder.AddQuantic(opt =>
            {
                opt.Assemblies = new System.Reflection.Assembly[]
                {
                    typeof(TestCompositonCommandHandler).Assembly
                };
            }).AddValidationDecorator();

            var container = builder.BuildServiceProvider();
            
            // Act
            var commandHandler = container.GetService<ICommandHandler<TestCompositonCommand>>();

            // Assert
            Assert.NotNull(commandHandler);
            Assert.Equal(expectedType, commandHandler.GetType().GetGenericTypeDefinition());
        }

        [Fact]
        public void ShouldRegisterValidationDecoratorForCommandHandlers()
        {
            // Arrange
            var expectedType = typeof(ValidationCommandHandlerDecorator<>).GetGenericTypeDefinition();
            var builder = new ServiceCollection();

            builder.AddQuantic(opt =>
            {
                opt.Assemblies = new System.Reflection.Assembly[]
                {
                    typeof(TestCompositonCommandHandler).Assembly
                };
            }).AddValidationDecorator(opt =>
            {
                opt.Assemblies = new System.Reflection.Assembly[]
                {
                    typeof(TestCompositonCommandHandler).Assembly
                };
            });

            var container = builder.BuildServiceProvider();

            // Act
            var commandHandler = container.GetService<ICommandHandler<TestCompositonCommand>>();

            // Assert
            Assert.NotNull(commandHandler);
            Assert.Equal(expectedType, commandHandler.GetType().GetGenericTypeDefinition());
        }

        [Fact]
        public void ShouldNotRegisterValidationDecoratorForCommandHandlersWithoutEnableValidatorAttribute()
        {
            // Arrange
            var expectedType = typeof(TestCompositonWithoutValidationCommandHandler);

            var builder = new ServiceCollection();

            builder.AddQuantic(opt =>
            {
                opt.Assemblies = new System.Reflection.Assembly[]
                {
                    typeof(TestCompositonCommandHandler).Assembly
                };
            }).AddValidationDecorator();

            var container = builder.BuildServiceProvider();
            
            // Act
            var commandHandler = container.GetService<ICommandHandler<TestCompositonWithoutValidationCommand>>();

            // Assert
            Assert.Equal(expectedType, commandHandler.GetType());
        }

        [Fact]
        public void ShouldRegisterCommandNalidators()
        {
            // Arrange
            var expectedType = typeof(TestCompositonCommandValidator);

            var builder = new ServiceCollection();

            builder.AddQuantic(opt =>
            {
                opt.Assemblies = new System.Reflection.Assembly[]
                {
                    typeof(TestCompositonCommandHandler).Assembly
                };
            }).AddValidationDecorator();

            var container = builder.BuildServiceProvider();

            // Act
            var validator = container.GetService<QuanticValidator<TestCompositonCommand>>();
            
            // Assert
            Assert.NotNull(validator);
            Assert.Equal(expectedType, validator.GetType());
        }


        [Fact]
        public void ShouldRegisterValidationDecoratorForQueryHandlers()
        {
            // Arrange
            var expectedType = typeof(ValidationQueryHandlerDecorator<,>).GetGenericTypeDefinition();

            var builder = new ServiceCollection();

            builder.AddQuantic(opt =>
            {
                opt.Assemblies = new System.Reflection.Assembly[]
                {
                    typeof(TestCompositonQueryHandler).Assembly
                };
            }).AddValidationDecorator();

            var container = builder.BuildServiceProvider();

            // Act
            var queryHandler = container.GetService<IQueryHandler<TestCompositonQuery,string>>();
            
            // Assert
            Assert.NotNull(queryHandler);
            Assert.Equal(expectedType, queryHandler.GetType().GetGenericTypeDefinition());
        }

        [Fact]
        public void ShouldNotRegisterValidationDecoratorForQueryHandlersWithoutEnableValidatorAttribute()
        {
            // Arrange
            var expectedType = typeof(TestCompositonWithoutValidationQueryHandler);
            var builder = new ServiceCollection();

            builder.AddQuantic(opt =>
            {
                opt.Assemblies = new System.Reflection.Assembly[]
                {
                    typeof(TestCompositonQueryHandler).Assembly
                };
            }).AddValidationDecorator();

            var container = builder.BuildServiceProvider();
            
            // Act
            var queryHandler = container.GetService<IQueryHandler<TestCompositonWithoutValidationQuery,string>>();

            // Assert
            Assert.Equal(expectedType, queryHandler.GetType());
        }

        [Fact]
        public void ShouldRegisterQueryValidators()
        {
            // Arrange
            var expectedType = typeof(TestCompositonQueryValidator);
            var builder = new ServiceCollection();

            builder.AddQuantic(opt =>
            {
                opt.Assemblies = new System.Reflection.Assembly[]
                {
                    typeof(TestCompositonQueryHandler).Assembly
                };
            }).AddValidationDecorator();

            var container = builder.BuildServiceProvider();

            // Act
            var validator = container.GetService<QuanticValidator<TestCompositonQuery>>();
            
            // Assert
            Assert.NotNull(validator);
            Assert.Equal(expectedType, validator.GetType());
        }

        public class TestCompositonCommand : ICommand 
        {
            public string CardNumber { get; set; }
            public DateTime Expire { get; set; }
        }

        [DecorateValidation]
        public class TestCompositonCommandHandler : ICommandHandler<TestCompositonCommand>
        {
            public Task<CommandResult> Handle(TestCompositonCommand command, RequestContext context)
            {
                return Task.FromResult(CommandResult.Success);
            }
        }

        public class TestCompositonCommandValidator : QuanticValidator<TestCompositonCommand>{ }



        public class TestCompositonWithoutValidationCommand : ICommand { }

        public class TestCompositonWithoutValidationCommandHandler : ICommandHandler<TestCompositonWithoutValidationCommand>
        {
            public Task<CommandResult> Handle(TestCompositonWithoutValidationCommand command, RequestContext context)
            {
                return Task.FromResult(CommandResult.Success);
            }
        }


        public class TestCompositonQuery : IQuery<string> 
        {
            public string CardNumber { get; set; }
            public DateTime Expire { get; set; }
        }

        [DecorateValidation]
        public class TestCompositonQueryHandler : IQueryHandler<TestCompositonQuery, string>
        {
            public Task<QueryResult<string>> Handle(TestCompositonQuery query, RequestContext context)
            {
                return Task.FromResult(new QueryResult<string>("ok"));
            }
        }

        public class TestCompositonQueryValidator : QuanticValidator<TestCompositonQuery> { }

        public class TestCompositonWithoutValidationQuery : IQuery<string> { }

        public class TestCompositonWithoutValidationQueryHandler : IQueryHandler<TestCompositonWithoutValidationQuery, string>
        {
            public Task<QueryResult<string>> Handle(TestCompositonWithoutValidationQuery query, RequestContext context)
            {
                return Task.FromResult(new QueryResult<string>("ok"));
            }
        }
    }     
}