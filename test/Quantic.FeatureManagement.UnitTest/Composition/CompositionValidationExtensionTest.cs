using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Quantic.Core;
using Xunit;

namespace Quantic.FeatureManagement.UnitTest
{
    public class CompositionFeatureManagementExtensionTest
    {
        [Fact]
        public void ShouldRegisterValidationDecoratorForCommandHandlers()
        {
            // Arrange
            var expectedType = typeof(FeatureManagementCommandHandlerDecorator<>).GetGenericTypeDefinition();

            var builder = new ServiceCollection();

            builder.AddQuantic(opt =>
            {
                opt.Assemblies = new System.Reflection.Assembly[]
                {
                    typeof(TestCompositonCommandHandler).Assembly
                };
            }).AddFeatureManagementDecorator();
            
            var holder = new SettingsHolder();
            builder.AddSingleton(holder);

            var container = builder.BuildServiceProvider();
            
            // Act
            var commandHandler = container.GetService<ICommandHandler<TestCompositonWithFeatureCommand>>();

            // Assert
            Assert.NotNull(commandHandler);
            Assert.Equal(expectedType, commandHandler.GetType().GetGenericTypeDefinition());
        }

        [Fact]
        public void ShouldNotRegisterDecoratorForHandlersWithoutEnableAttribute()
        {
            // Arrange

            var builder = new ServiceCollection();

            builder.AddQuantic(opt =>
            {
                opt.Assemblies = new System.Reflection.Assembly[]
                {
                    typeof(TestCompositonCommandHandler).Assembly
                };
            }).AddFeatureManagementDecorator();
            
            var holder = new SettingsHolder();
            builder.AddSingleton(holder);

            var container = builder.BuildServiceProvider();
            
            // Act
            var commandHandler = container.GetService<ICommandHandler<TestCompositonCommand>>();

            // Assert
            Assert.IsType<TestCompositonCommandHandler>(commandHandler);
        }

        [Fact]
        public void ShouldRegisterDecoratorForQueryHandlers()
        {
            // Arrange
            var expectedType = typeof(FeatureManagementQueryHandlerDecorator<,>).GetGenericTypeDefinition();

            var builder = new ServiceCollection();

            builder.AddQuantic(opt =>
            {
                opt.Assemblies = new System.Reflection.Assembly[]
                {
                    typeof(TestCompositonQueryHandler).Assembly
                };
            }).AddFeatureManagementDecorator();

            var holder = new SettingsHolder();
            builder.AddSingleton(holder);
            
            var container = builder.BuildServiceProvider();

            // Act
            var queryHandler = container.GetService<IQueryHandler<TestCompositonWithFeatureQuery,string>>();
            
            // Assert
            Assert.NotNull(queryHandler);
            Assert.Equal(expectedType, queryHandler.GetType().GetGenericTypeDefinition());
        }

        [Fact]
        public void ShouldNotRegisterDecoratorForQueryHandlersWithoutEnableAttribute()
        {
            // Arrange
            var builder = new ServiceCollection();

            builder.AddQuantic(opt =>
            {
                opt.Assemblies = new System.Reflection.Assembly[]
                {
                    typeof(TestCompositonQueryHandler).Assembly
                };
            }).AddFeatureManagementDecorator();
            
            var holder = new SettingsHolder();
            builder.AddSingleton(holder);

            var container = builder.BuildServiceProvider();
            
            // Act
            var queryHandler = container.GetService<IQueryHandler<TestCompositonQuery,string>>();

            // Assert
            Assert.IsType<TestCompositonQueryHandler>(queryHandler);
        }

        public class TestCompositonWithFeatureCommand : ICommand { }
       
        [DecorateFeatureManagement("FeatureA")]        
        public class TestCompositonWithFeatureCommandHandler : ICommandHandler<TestCompositonWithFeatureCommand>
        {
            public Task<CommandResult> Handle(TestCompositonWithFeatureCommand command, RequestContext context)
            {
                return Task.FromResult(CommandResult.Success);
            }
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
            public string CardNumber { get; set; }
            public DateTime Expire { get; set; }
        }


        public class TestCompositonQueryHandler : IQueryHandler<TestCompositonQuery, string>
        {
            public Task<QueryResult<string>> Handle(TestCompositonQuery query, RequestContext context)
            {
                return Task.FromResult(new QueryResult<string>("ok"));
            }
        }

        public class TestCompositonWithFeatureQuery : IQuery<string> { }
        
        [DecorateFeatureManagement("FeatureA")]
        public class TestCompositonWithFeatureQueryHandler : IQueryHandler<TestCompositonWithFeatureQuery, string>
        {
            public Task<QueryResult<string>> Handle(TestCompositonWithFeatureQuery query, RequestContext context)
            {
                return Task.FromResult(new QueryResult<string>("ok"));
            }
        }
    }     
}