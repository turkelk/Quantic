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
    public sealed class LogRedactionQueryDecoratorTest
    {
        [Fact]
        public async Task Redacts_Configured_Request_Fields_Recursively()
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
                        Name = typeof(RedactionQuery).Name,
                        RedactRequestProperties = new[] { "Base64", "Token" }
                    }
                }
            };

            RequestLog captured = null;
            var mockRequestLogger = new Mock<IRequestLogger>();
            mockRequestLogger
                .Setup(x => x.Log(It.IsAny<RequestLog>()))
                .Callback<RequestLog>(l => captured = l)
                .Returns(Task.CompletedTask);

            var mockLogger = new Mock<ILogger<RedactionQuery>>();
            var mockQueryHandler = new Mock<IQueryHandler<RedactionQuery, string>>();

            var query = new RedactionQuery
            {
                ApiKey = "SHOULD_NOT_LOG",
                Keep = "keep_me",
                Payload = new Payload
                {
                    Base64 = "AAA",
                    Nested = new Nested { Token = "BBB", Keep = "nested_keep" }
                }
            };

            mockQueryHandler
                .Setup(x => x.Handle(query, It.IsAny<RequestContext>()))
                .ReturnsAsync(new QueryResult<string>("OK"));

            var decorator = new LogQueryHandlerDecorator<RedactionQuery, string>(
                mockRequestLogger.Object,
                mockQueryHandler.Object,
                logSettings,
                mockLogger.Object);

            // Act
            var result = await decorator.Handle(query, Helper.Context);

            // Assert
            Assert.NotNull(captured);
            Assert.Equal(Messages.Success, result.Code);

            var req = Assert.IsAssignableFrom<JsonNode>(captured.Request);
            var obj = req.AsObject();

            Assert.Equal("***", obj["ApiKey"]?.ToString());
            Assert.Equal("keep_me", obj["Keep"]?.ToString());

            var payload = obj["Payload"]!.AsObject();
            Assert.Equal("***", payload["Base64"]?.ToString());
            Assert.Equal("nested_keep", payload["Nested"]!.AsObject()["Keep"]?.ToString());
            Assert.Equal("***", payload["Nested"]!.AsObject()["Token"]?.ToString());

            mockRequestLogger.Verify(x => x.Log(It.IsAny<RequestLog>()), Times.Once);
        }

        [Fact]
        public async Task Redacts_Configured_Response_Fields()
        {
            // Arrange
            var logSettings = new LogSettings
            {
                RedactionMask = "***",
                Settings = new[]
                {
                    new LogSetting
                    {
                        Name = typeof(RedactionQuery).Name,
                        RedactResponseProperties = new[] { "Result" }
                    }
                }
            };

            RequestLog captured = null;
            var mockRequestLogger = new Mock<IRequestLogger>();
            mockRequestLogger
                .Setup(x => x.Log(It.IsAny<RequestLog>()))
                .Callback<RequestLog>(l => captured = l)
                .Returns(Task.CompletedTask);

            var mockLogger = new Mock<ILogger<RedactionQuery>>();
            var mockQueryHandler = new Mock<IQueryHandler<RedactionQuery, string>>();

            var query = new RedactionQuery();
            mockQueryHandler
                .Setup(x => x.Handle(query, It.IsAny<RequestContext>()))
                .ReturnsAsync(new QueryResult<string>("SECRET_RESULT"));

            var decorator = new LogQueryHandlerDecorator<RedactionQuery, string>(
                mockRequestLogger.Object,
                mockQueryHandler.Object,
                logSettings,
                mockLogger.Object);

            // Act
            var result = await decorator.Handle(query, Helper.Context);

            // Assert
            Assert.Equal(Messages.Success, result.Code);
            Assert.NotNull(captured);

            var resp = Assert.IsAssignableFrom<JsonNode>(captured.Response);
            var obj = resp.AsObject();
            Assert.Equal("***", obj["Result"]?.ToString());
        }

        public sealed class RedactionQuery : IQuery<string>
        {
            public string ApiKey { get; set; }
            public string Keep { get; set; }
            public Payload Payload { get; set; }
        }

        public sealed class Payload
        {
            public string Base64 { get; set; }
            public Nested Nested { get; set; }
        }

        public sealed class Nested
        {
            public string Token { get; set; }
            public string Keep { get; set; }
        }
    }
}

