using System.Threading.Tasks;
using FluentValidation;
using Moq;
using Quantic.Core;
using Xunit;

namespace Quantic.Validation.UnitTest.Main
{
    public class ValidationQueryHandlerDecoratorTest
    {
        [Fact]
        public async Task ShouldReturnFailure()
        {
            // Arrange         
            var request = new TestQuery();
            var mockHandler = new Mock<IQueryHandler<TestQuery, string>>();

            // Act
            var decorator = new ValidationQueryHandlerDecorator<TestQuery, string>(mockHandler.Object, new TestQuerValidator());
            var result = await decorator.Handle(request, Helper.Context);

            //Assert
            Assert.True(result.HasError);
            Assert.Equal("error_code", result.Errors[0].Code);
        } 

        [Fact]
        public async Task ShouldNotCallDecoratedHandler()
        {
            // Arrange         
            var request = new TestQuery();
            var mockHandler = new Mock<IQueryHandler<TestQuery, string>>();

            // Act
            var decorator = new ValidationQueryHandlerDecorator<TestQuery, string>(mockHandler.Object, new TestQuerValidator());
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
            var request = new TestQuery
            {
                 NotNullableValue = "value"
            };

            var mockHandler = new Mock<IQueryHandler<TestQuery, string>>();
            mockHandler
            .Setup(x=>x.Handle(request, Helper.Context))
            .ReturnsAsync(new QueryResult<string>("OK"));

            // Act
            var decorator = new ValidationQueryHandlerDecorator<TestQuery, string>(mockHandler.Object, new TestQuerValidator());
            var result = await decorator.Handle(request, Helper.Context);

            //Assert
            Assert.True(result.IsSuccess);
            mockHandler.Verify(x=>x.Handle(request, Helper.Context),Times.Once);
        }                 

        private class TestQuery : IQuery<string>
        {
            public string NotNullableValue;
        }

        [DecorateValidation]
        private class TestQueryHandler : IQueryHandler<TestQuery, string>
        {
            public Task<QueryResult<string>> Handle(TestQuery query, RequestContext context)
            {
                return Task.FromResult(new QueryResult<string>("OK"));
            }
        }

        private class TestQuerValidator : QuanticValidator<TestQuery>
        {
            public TestQuerValidator()
            {
                RuleFor(x => x.NotNullableValue)
                    .NotNull()
                    .WithErrorCode("error_code")
                    .WithMessage("error_message");
            }
        }                     
    }
}