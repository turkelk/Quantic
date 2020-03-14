using System.Threading.Tasks;
using FluentValidation;
using Moq;
using Quantic.Core;
using Xunit;

namespace Quantic.Validation.UnitTest.Main
{
    public class ValidationCommandHandlerDecoratorTest
    {
        [Fact]
        public async Task ShouldReturnFailure()
        {
            // Arrange         
            var request = new TestCommand();
            var mockHandler = new Mock<ICommandHandler<TestCommand>>();

            // Act
            var decorator = new ValidationCommandHandlerDecorator<TestCommand>(mockHandler.Object, new TestCommandValidator());
            var result = await decorator.Handle(request, Helper.Context);

            //Assert
            Assert.True(result.HasError);
            Assert.Equal("error_code", result.Errors[0].Code);
        } 

        [Fact]
        public async Task ShouldNotCallDecoratedHandler()
        {
            // Arrange         
            var request = new TestCommand();
            var mockHandler = new Mock<ICommandHandler<TestCommand>>();           

            // Act
            var decorator = new ValidationCommandHandlerDecorator<TestCommand>(mockHandler.Object, new TestCommandValidator());
            var result = await decorator.Handle(request, Helper.Context);

            //Assert
            Assert.True(result.HasError);
            Assert.Equal("error_code", result.Errors[0].Code);
            mockHandler.Verify(x=>x.Handle(request,Helper.Context),Times.Never);
        }

        [Fact]
        public async Task ShouldCallDecoratorOnValidationSuccess()
        {
            // Arrange         
            var request = new TestCommand
            {
                 NotNullableValue = "value"
            };

            var mockHandler = new Mock<ICommandHandler<TestCommand>>(); 
            mockHandler
            .Setup(x=>x.Handle(request, Helper.Context))
            .ReturnsAsync(CommandResult.Success);

            // Act
            var decorator = new ValidationCommandHandlerDecorator<TestCommand>(mockHandler.Object, new TestCommandValidator());
            var result = await decorator.Handle(request, Helper.Context);

            //Assert
            Assert.True(result.IsSuccess);
            mockHandler.Verify(x=>x.Handle(request, Helper.Context),Times.Once);
        }                 

        [DecorateValidation]
        public class TestCommand : ICommand
        {
            public string NotNullableValue;
        }

        public class TestCommandHandler : ICommandHandler<TestCommand>
        {
            public Task<CommandResult> Handle(TestCommand command, RequestContext context)
            {
                return Task.FromResult(CommandResult.Success);
            }
        }

        public class TestCommandValidator : QuanticValidator<TestCommand>
        {
            public TestCommandValidator()
            {
                RuleFor(x => x.NotNullableValue)
                    .NotNull()
                    .WithErrorCode("error_code")
                    .WithMessage("error message");
            }
        }                      
    }
}