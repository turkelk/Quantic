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

        [Fact]
        public async Task Supports_Dotted_Paths_And_Deep_Wildcards_For_Request_Redaction()
        {
            // Arrange
            var logSettings = new LogSettings
            {
                RedactionMask = "***",
                Settings = new[]
                {
                    new LogSetting
                    {
                        Name = typeof(PathRedactionCommand).Name,
                        // redact Token anywhere under Payload (Payload.**.Token)
                        // redact only Payload.Nested.Base64
                        RedactRequestProperties = new[] { "Payload.**.Token", "Payload.Nested.Base64" }
                    }
                }
            };

            RequestLog captured = null;
            var mockRequestLogger = new Mock<IRequestLogger>();
            mockRequestLogger
                .Setup(x => x.Log(It.IsAny<RequestLog>()))
                .Callback<RequestLog>(l => captured = l)
                .Returns(Task.CompletedTask);

            var mockLogger = new Mock<ILogger<PathRedactionCommand>>();
            var mockHandler = new Mock<ICommandHandler<PathRedactionCommand>>();

            var cmd = new PathRedactionCommand
            {
                Keep = "ok",
                Payload = new PathPayload
                {
                    Nested = new PathNested
                    {
                        Base64 = "NESTED_B64",
                        Token = "NESTED_TOKEN",
                        Deep = new PathDeep { Token = "DEEP_TOKEN" }
                    },
                    Other = new PathNested
                    {
                        Base64 = "OTHER_B64",
                        Token = "OTHER_TOKEN",
                        Deep = new PathDeep { Token = "OTHER_DEEP_TOKEN" }
                    }
                }
            };

            mockHandler
                .Setup(x => x.Handle(cmd, It.IsAny<RequestContext>()))
                .ReturnsAsync(CommandResult.Success);

            var decorator = new LogCommandHandlerDecorator<PathRedactionCommand>(
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
            Assert.Equal("ok", obj["Keep"]?.ToString());

            var payload = obj["Payload"]!.AsObject();
            var nested = payload["Nested"]!.AsObject();
            Assert.Equal("***", nested["Base64"]?.ToString());
            Assert.Equal("***", nested["Token"]?.ToString());
            Assert.Equal("***", nested["Deep"]!.AsObject()["Token"]?.ToString());

            var other = payload["Other"]!.AsObject();
            Assert.Equal("OTHER_B64", other["Base64"]?.ToString());
            Assert.Equal("***", other["Token"]?.ToString());
            Assert.Equal("***", other["Deep"]!.AsObject()["Token"]?.ToString());
        }

        public sealed class RedactionCommand : ICommand
        {
            public string ApiKey { get; set; }
            public string Password { get; set; }
            public string Keep { get; set; }
        }

        public sealed class PathRedactionCommand : ICommand
        {
            public string Keep { get; set; }
            public PathPayload Payload { get; set; }
        }

        public sealed class PathPayload
        {
            public PathNested Nested { get; set; }
            public PathNested Other { get; set; }
        }

        public sealed class PathNested
        {
            public string Base64 { get; set; }
            public string Token { get; set; }
            public PathDeep Deep { get; set; }
        }

        public sealed class PathDeep
        {
            public string Token { get; set; }
        }
    }
}

