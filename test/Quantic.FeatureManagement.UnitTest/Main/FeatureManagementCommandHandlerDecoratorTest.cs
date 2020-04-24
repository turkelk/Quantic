using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Quantic.Core;
using Xunit;

namespace Quantic.FeatureManagement.UnitTest
{
    public class FeatureManagementCommandHandlerDecoratorTest
    {
        Mock<IHandlerFeatureInfoProvider> mockProvider;
        Mock<SettingsHolder> mockSettingsHolder;
        Mock<ICommandHandler<TestCommand>> mockCommandHandler;
        public FeatureManagementCommandHandlerDecoratorTest()
        {
            mockProvider = new Mock<IHandlerFeatureInfoProvider>();
            mockSettingsHolder = new Mock<SettingsHolder>();
            mockCommandHandler = new Mock<ICommandHandler<TestCommand>>();
        }

        [Fact]
        public async Task ShoulCallDecoratedHandlerIfHandlerHasNotFeature()
        {
            // Arrange            
            mockProvider.Setup(x => x.GetHandlerInfo(It.IsAny<string>()))
            .Returns(default(HandlerFeatureInfo));

            var command = new TestCommand();

            // Assert
            mockCommandHandler
            .Verify(x => x.Handle(command, It.IsAny<RequestContext>()), Times.AtMostOnce);

            var decodator = new FeatureManagementCommandHandlerDecorator<TestCommand>(mockCommandHandler.Object, mockSettingsHolder.Object, mockProvider.Object);

            // Act
            await decodator.Handle(command, Helper.Context);
        }

        // [Fact]
        // public async Task ShoulCallDecoratedHandlerIfFeaturesUsed()
        // {
        //     // Arrange    
        //     var featureC = "FeatureC";
        //     var featureQ = "FeatureQ";
        //     var features = new string[] { featureC, featureQ };
        //     var featureInfo = new HandlerFeatureInfo(typeof(TestCommand).Name, features);

        //     mockProvider.Setup(x => x.GetHandlerInfo(It.IsAny<string>()))
        //     .Returns(featureInfo);

        //     var command = new TestCommand();

        //     // Assert
        //     mockCommandHandler
        //     .Verify(x => x.Handle(command, It.IsAny<RequestContext>()), Times.AtMostOnce);

        //     var decodator = new FeatureManagementCommandHandlerDecorator<TestCommand>(mockCommandHandler.Object, mockSettingsHolder.Object, mockProvider.Object);

        //     var headers = new Dictionary<string, string>
        //     {
        //         {$"{FeatureHeader.Prefix}-{featureC}", "SomeRequest"},
        //         {$"{FeatureHeader.Prefix}-{featureQ}", "SomeRequest"}
        //     };
        //     var context = new RequestContext("trace-id", headers);

        //     // Act
        //     await decodator.Handle(command, context);
        // }

        [Fact]
        public async Task ShoulNotCallDecoratedHandlerIfAllFeaturesNotEnabled()
        {
            // Arrange    
            var featureC = "FeatureC";
            var featureQ = "FeatureQ";
            var features = new string[] { featureC, featureQ };
            var featureInfo = new HandlerFeatureInfo(typeof(TestCommand).Name, features);

            mockProvider.Setup(x => x.GetHandlerInfo(It.IsAny<string>()))
            .Returns(featureInfo);

            var settingsHolder = new SettingsHolder();
            settingsHolder.Settings = new Setting[]
            {
                new Setting
                {
                    FeatureName = featureC,
                    Enable = false
                },
               new Setting
                {
                    FeatureName = featureQ,
                    Enable = true
                }
            };

            var command = new TestCommand();

            // Assert
            mockCommandHandler
            .Verify(x => x.Handle(command, It.IsAny<RequestContext>()), Times.Never);

            var decodator = new FeatureManagementCommandHandlerDecorator<TestCommand>(mockCommandHandler.Object, settingsHolder, mockProvider.Object);

            // Act
            var result = await decodator.Handle(command, Helper.Context);

            // Assert
            Assert.Equal(Msg.FeatureNotEnabled,result.Code);
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task ShoulCallDecoratedHandlerAndAddUsedFeaturesToHeader()
        {
            // Arrange    
            var featureC = "FeatureC";
            var featureQ = "FeatureQ";
            var features = new string[] { featureC, featureQ };
            var featureInfo = new HandlerFeatureInfo(typeof(TestCommand).Name, features);

            mockProvider.Setup(x => x.GetHandlerInfo(It.IsAny<string>()))
            .Returns(featureInfo);

            var settingsHolder = new SettingsHolder();
            settingsHolder.Settings = new Setting[]
            {
                new Setting
                {
                    FeatureName = featureC,
                    Enable = true
                },
               new Setting
                {
                    FeatureName = featureQ,
                    Enable = true
                }
            };

            var command = new TestCommand();

            // Assert
            mockCommandHandler
            .Setup(x => x.Handle(command, It.IsAny<RequestContext>()))
            .ReturnsAsync(CommandResult.Success);

            var decodator = new FeatureManagementCommandHandlerDecorator<TestCommand>(mockCommandHandler.Object, settingsHolder, mockProvider.Object);

            // Act
            var result = await decodator.Handle(command, Helper.Context);

            // Assert
            Assert.True(result.IsSuccess);
            // Assert.True(Helper.Context.Headers.ContainsKey($"{FeatureHeader.Prefix}-{featureC}"));
            // Assert.True(Helper.Context.Headers.ContainsKey($"{FeatureHeader.Prefix}-{featureQ}"));
        }        
    }
}