using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Quantic.Core;
using Xunit;

namespace Quantic.FeatureManagement.UnitTest
{
    public class FeatureManagementQueryHandlerDecoratorTest
    {
        Mock<IHandlerFeatureInfoProvider> mockProvider;
        Mock<FeatureSettingsHolder> mockSettingsHolder;
        Mock<IQueryHandler<TestQuery,string>> mockHandler;
        public FeatureManagementQueryHandlerDecoratorTest()
        {
            mockProvider = new Mock<IHandlerFeatureInfoProvider>();
            mockSettingsHolder = new Mock<FeatureSettingsHolder>();
            mockHandler = new Mock<IQueryHandler<TestQuery, string>>();
        }

        [Fact]
        public async Task ShoulCallDecoratedHandlerIfHandlerHasNotFeature()
        {
            // Arrange            
            mockProvider.Setup(x => x.GetHandlerInfo(It.IsAny<string>()))
            .Returns(default(HandlerFeatureInfo));

            var query = new TestQuery();

            // Assert
            mockHandler
            .Verify(x => x.Handle(query, It.IsAny<RequestContext>()), Times.AtMostOnce);

            var decodator = new FeatureManagementQueryHandlerDecorator<TestQuery,string>(mockHandler.Object, mockSettingsHolder.Object, mockProvider.Object);

            // Act
            await decodator.Handle(query, Helper.Context);
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

        //     var query = new TestQuery();

        //     // Assert
        //     mockHandler
        //     .Verify(x => x.Handle(query, It.IsAny<RequestContext>()), Times.AtMostOnce);

        //     var decodator = new FeatureManagementQueryHandlerDecorator<TestQuery,string>(mockHandler.Object, mockSettingsHolder.Object, mockProvider.Object);

        //     var headers = new Dictionary<string, string>
        //     {
        //         {$"{FeatureHeader.Prefix}-{featureC}", "SomeRequest"},
        //         {$"{FeatureHeader.Prefix}-{featureQ}", "SomeRequest"}
        //     };
        //     var context = new RequestContext("trace-id", headers);

        //     // Act
        //     await decodator.Handle(query, context);
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

            var settingsHolder = new FeatureSettingsHolder();
            settingsHolder.Settings = new FeatureSetting[]
            {
                new FeatureSetting
                {
                    FeatureName = featureC,
                    Enable = false
                },
               new FeatureSetting
                {
                    FeatureName = featureQ,
                    Enable = true
                }
            };

            var query = new TestQuery();

            // Assert
            mockHandler
            .Verify(x => x.Handle(query, It.IsAny<RequestContext>()), Times.Never);

            var decodator = new FeatureManagementQueryHandlerDecorator<TestQuery,string>(mockHandler.Object, 
                settingsHolder, mockProvider.Object);

            // Act
            var result = await decodator.Handle(query, Helper.Context);

            // Assert
            Assert.Equal(FeatureMessages.FeatureNotEnabled,result.Code);
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

            var settingsHolder = new FeatureSettingsHolder();
            settingsHolder.Settings = new FeatureSetting[]
            {
                new FeatureSetting
                {
                    FeatureName = featureC,
                    Enable = true
                },
               new FeatureSetting
                {
                    FeatureName = featureQ,
                    Enable = true
                }
            };

            var query = new TestQuery();

            // Assert
            mockHandler
            .Setup(x => x.Handle(query, It.IsAny<RequestContext>()))
            .ReturnsAsync(QueryResult<string>.WithResult(""));

            var decodator = new FeatureManagementQueryHandlerDecorator<TestQuery,string>(mockHandler.Object, 
                settingsHolder, mockProvider.Object);

            // Act
            var result = await decodator.Handle(query, Helper.Context);

            // Assert
            Assert.True(result.IsSuccess);
        }        
    }
}