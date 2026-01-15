using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Quantic.Core;
using Quantic.Log;
using Quantic.Log.UnitTest;
using Xunit;

namespace Genesis.Log.Test
{
    public class LogQueryHandlerDecoratorTest
    {
        [Fact]
        public async Task ShouldLogIfSettingsIsNotProvided()
        {
            // Arrange
            LogSettings logSettings = new LogSettings();

            Mock<IRequestLogger> mockRequestLogger = new Mock<IRequestLogger>();
            Mock<ILogger<TestQuery>> mockLogger = new Mock<ILogger<TestQuery>>();
            Mock<IQueryHandler<TestQuery, string>> mockQueryHandler = new Mock<IQueryHandler<TestQuery, string>>();
            var query = new TestQuery();

            mockQueryHandler
                .Setup(x => x.Handle(query, It.IsAny<RequestContext>()))
                .ReturnsAsync(new QueryResult<string>("OK"));

            var decorator = new LogQueryHandlerDecorator<TestQuery, string>(
                mockRequestLogger.Object,
                mockQueryHandler.Object,
                logSettings,
                mockLogger.Object);

            // Act
            var result = await decorator.Handle(query, Helper.Context);

            // Assert
            mockRequestLogger.Verify(x => x.Log(It.IsAny<RequestLog>()), Times.Exactly(1));
            Assert.Equal(Messages.Success, result.Code);
        }

        [Fact]
        public async Task ShouldLogIfQueryIsNotInExcludeList()
        {
            // Arrange
            LogSettings logSettings = new LogSettings();

            Mock<IRequestLogger> mockRequestLogger = new Mock<IRequestLogger>();
            Mock<ILogger<TestQuery>> mockLogger = new Mock<ILogger<TestQuery>>();
            Mock<IQueryHandler<TestQuery, string>> mockQueryHandler = new Mock<IQueryHandler<TestQuery, string>>();
            var query = new TestQuery();

            mockQueryHandler
                .Setup(x => x.Handle(query, It.IsAny<RequestContext>()))
                .ReturnsAsync(new QueryResult<string>("OK"));

            var decorator = new LogQueryHandlerDecorator<TestQuery, string>(
                mockRequestLogger.Object,
                mockQueryHandler.Object,
                logSettings,
                mockLogger.Object);

            // Act
            var result = await decorator.Handle(query, Helper.Context);

            // Assert
            mockRequestLogger.Verify(x => x.Log(It.IsAny<RequestLog>()), Times.Exactly(1));
            Assert.Equal(Messages.Success, result.Code);
        }

        [Fact]
        public async Task ShouldNotLogIfQueryIsInExcludeList()
        {
            // Arrange
            LogSettings logSettings = new LogSettings
            {
                Settings = new LogSetting[] { new LogSetting { ShouldLog = false, Name = typeof(TestQuery).Name } }
            };

            Mock<IRequestLogger> mockRequestLogger = new Mock<IRequestLogger>();
            Mock<ILogger<TestQuery>> mockLogger = new Mock<ILogger<TestQuery>>();
            Mock<IQueryHandler<TestQuery, string>> mockQueryHandler = new Mock<IQueryHandler<TestQuery, string>>();
            var query = new TestQuery();

            mockQueryHandler
                .Setup(x => x.Handle(query, It.IsAny<RequestContext>()))
                .ReturnsAsync(new QueryResult<string>("OK"));

            var decorator = new LogQueryHandlerDecorator<TestQuery, string>(
                mockRequestLogger.Object,
                mockQueryHandler.Object,
                logSettings,
                mockLogger.Object);

            // Act
            var result = await decorator.Handle(query, Helper.Context);

            // Assert
            mockRequestLogger.Verify(x => x.Log(It.IsAny<RequestLog>()), Times.Exactly(0));
            Assert.Equal(Messages.Success, result.Code);
        }

        public class TestQuery : IQuery<string> { }
    }
}
