using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Quantic.Core;
using Quantic.Log;
using Quantic.Log.UnitTest;
using Xunit;

namespace Genesis.Log.Test
{
    public sealed class LogRedactionCommandDecoratorTest
    {
        [Fact]
        public async Task Redacts_Global_And_PerCommand_Request_Fields()
        {
            // Arrange
            var logSettings = new LogSettings
            {
                RedactionMask = "***",
                GlobalRedactProperties = new[] { "ApiKey" },
                Settings = new[]
                {
                    new LogSetting
                    {
                        Name = typeof(RedactionCommand).Name,
                        RedactRequestProperties = new[] { "Password" }
                    }
                }
            };

            RequestLog captured = null;
            var mockRequestLogger = new Mock<IRequestLogger>();
            mockRequestLogger
                .Setup(x => x.Log(It.IsAny<RequestLog>()))
                .Callback<RequestLog>(l => captured = l)
                .Returns(Task.CompletedTask);

            var mockLogger = new Mock<ILogger<RedactionCommand>>();
            var mockHandler = new Mock<ICommandHandler<RedactionCommand>>();

            var cmd = new RedactionCommand
            {
                ApiKey = "K",
                Password = "P",
                Keep = "ok"
            };

            mockHandler
                .Setup(x => x.Handle(cmd, It.IsAny<RequestContext>()))
                .ReturnsAsync(CommandResult.Success);

            var decorator = new LogCommandHandlerDecorator<RedactionCommand>(
                mockRequestLogger.Object,
                mockHandler.Object,
                logSettings,
                mockLogger.Object);

            // Act
            var result = await decorator.Handle(cmd, Helper.Context);

            // Assert
            Assert.Equal(Messages.Success, result.Code);
            Assert.NotNull(captured);

            var req = Assert.IsAssignableFrom<JsonNode>(captured.Request);
            var obj = req.AsObject();
            Assert.Equal("***", obj["ApiKey"]?.ToString());
            Assert.Equal("***", obj["Password"]?.ToString());
            Assert.Equal("ok", obj["Keep"]?.ToString());
        }

        [Fact]
        public async Task Redacts_Response_Fields_When_Configured()
        {
            // Arrange
            var logSettings = new LogSettings
            {
                RedactionMask = "***",
                Settings = new[]
                {
                    new LogSetting
                    {
                        Name = typeof(RedactionCommand).Name,
                        RedactResponseProperties = new[] { "Message" }
                    }
                }
            };

            RequestLog captured = null;
            var mockRequestLogger = new Mock<IRequestLogger>();
            mockRequestLogger
                .Setup(x => x.Log(It.IsAny<RequestLog>()))
                .Callback<RequestLog>(l => captured = l)
                .Returns(Task.CompletedTask);

            var mockLogger = new Mock<ILogger<RedactionCommand>>();
            var mockHandler = new Mock<ICommandHandler<RedactionCommand>>();

            var cmd = new RedactionCommand();
            mockHandler
                .Setup(x => x.Handle(cmd, It.IsAny<RequestContext>()))
                .ReturnsAsync(CommandResult.WithMessage("success", "SECRET_MESSAGE"));

            var decorator = new LogCommandHandlerDecorator<RedactionCommand>(
                mockRequestLogger.Object,
                mockHandler.Object,
                logSettings,
                mockLogger.Object);

            // Act
            var result = await decorator.Handle(cmd, Helper.Context);

            // Assert
            Assert.Equal("success", result.Code);
            Assert.NotNull(captured);

            var resp = Assert.IsAssignableFrom<JsonNode>(captured.Response);
            var obj = resp.AsObject();
            Assert.Equal("***", obj["Message"]?.ToString());
        }

        public sealed class RedactionCommand : ICommand
        {
            public string ApiKey { get; set; }
            public string Password { get; set; }
            public string Keep { get; set; }
        }
    }
}

