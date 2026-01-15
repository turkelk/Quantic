using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Quantic.Core;
using Quantic.Log;
using Quantic.Log.UnitTest;
using Xunit;

namespace Genesis.Log.Test
{
    public class LogCommandHandlerDecoratorTest
    {
        [Fact]
        public async Task ShouldLogIfSettingsIsNotProvided()
        {
            // Arrange
            LogSettings logSettings = new LogSettings();

            Mock<IRequestLogger> mockRequestLogger = new Mock<IRequestLogger>();
            Mock<ILogger<TestCommand>> mockLogger = new Mock<ILogger<TestCommand>>();
            Mock<ICommandHandler<TestCommand>> mockCommandHandler = new Mock<ICommandHandler<TestCommand>>();
            var command = new TestCommand();

            mockCommandHandler
                .Setup(x => x.Handle(command, It.IsAny<RequestContext>()))
                .ReturnsAsync(CommandResult.Success);

            var decorator = new LogCommandHandlerDecorator<TestCommand>(
                mockRequestLogger.Object,
                mockCommandHandler.Object,
                logSettings,
                mockLogger.Object);

            // Act
            var result = await decorator.Handle(command, Helper.Context);

            // Assert
            mockRequestLogger.Verify(x => x.Log(It.IsAny<RequestLog>()), Times.Exactly(1));
            Assert.Equal(Messages.Success, result.Code);
        }

        [Fact]
        public async Task ShouldLogIfCommandIsNotInExcludeList()
        {
            // Arrange
            LogSettings logSettings = new LogSettings { Settings = new LogSetting[] { } };

            Mock<IRequestLogger> mockRequestLogger = new Mock<IRequestLogger>();
            Mock<ILogger<TestCommand>> mockLogger = new Mock<ILogger<TestCommand>>();
            Mock<ICommandHandler<TestCommand>> mockCommandHandler = new Mock<ICommandHandler<TestCommand>>();
            var command = new TestCommand();

            mockCommandHandler
                .Setup(x => x.Handle(command, It.IsAny<RequestContext>()))
                .ReturnsAsync(CommandResult.Success);

            var decorator = new LogCommandHandlerDecorator<TestCommand>(
                mockRequestLogger.Object,
                mockCommandHandler.Object,
                logSettings,
                mockLogger.Object);

            // Act
            var result = await decorator.Handle(command, Helper.Context);

            // Assert
            mockRequestLogger.Verify(x => x.Log(It.IsAny<RequestLog>()), Times.Exactly(1));
            Assert.Equal(Messages.Success, result.Code);
        }

        [Fact]
        public async Task ShouldNotLogIfCommandIsInExcludeList()
        {
            // Arrange
            LogSettings logSettings = new LogSettings
            {
                Settings = new LogSetting[] { new LogSetting { ShouldLog = false, Name = typeof(TestCommand).Name } }
            };

            Mock<IRequestLogger> mockRequestLogger = new Mock<IRequestLogger>();
            Mock<ILogger<TestCommand>> mockLogger = new Mock<ILogger<TestCommand>>();
            Mock<ICommandHandler<TestCommand>> mockCommandHandler = new Mock<ICommandHandler<TestCommand>>();
            var command = new TestCommand();

            mockCommandHandler
                .Setup(x => x.Handle(command, It.IsAny<RequestContext>()))
                .ReturnsAsync(CommandResult.Success);

            var decorator = new LogCommandHandlerDecorator<TestCommand>(
                mockRequestLogger.Object,
                mockCommandHandler.Object,
                logSettings,
                mockLogger.Object);

            // Act
            var result = await decorator.Handle(command, Helper.Context);

            // Assert
            mockRequestLogger.Verify(x => x.Log(It.IsAny<RequestLog>()), Times.Exactly(0));
            Assert.Equal(Messages.Success, result.Code);
        }

        public class TestCommand : ICommand { }
    }
}
