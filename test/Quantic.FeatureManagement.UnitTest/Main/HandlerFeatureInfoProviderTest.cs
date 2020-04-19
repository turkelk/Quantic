using System;
using System.Linq;
using Xunit;

namespace Quantic.FeatureManagement.UnitTest
{
    public class HandlerFeatureInfoProviderTest
    {
        [Fact]
        public void ShouldSuccess()
        {
            // Arrange
            var types = new Type[] 
            {
                typeof(TestCommandHandler),
                typeof(TestQueryHandler),
                typeof(MultipleFeaturedTestCommandHandler)                
            };
            
            var provider = new HandlerFeatureInfoProvider(types);

            // Act
            var commandHandlerInfo = provider.GetHandlerInfo(typeof(TestCommand).Name);

            // Assert
            Assert.Equal(typeof(TestCommand).Name, commandHandlerInfo.Name);
            Assert.Single(commandHandlerInfo.Features);
            Assert.Contains("FeatureC", commandHandlerInfo.Features);

            // Act
            var queryHandlerInfo = provider.GetHandlerInfo(typeof(TestQuery).Name);

            // Assert
            Assert.Equal(typeof(TestQuery).Name, queryHandlerInfo.Name);
            Assert.Single(queryHandlerInfo.Features);
            Assert.Contains("FeatureQ", queryHandlerInfo.Features); 

            // Act
            var multipleFeatureHandler = provider.GetHandlerInfo(typeof(MultipleFeaturedTestCommand).Name);

            // Assert
            Assert.Equal(typeof(MultipleFeaturedTestCommand).Name, multipleFeatureHandler.Name);
            Assert.Equal(2, multipleFeatureHandler.Features.Count());
            Assert.Contains("FeatureQ", multipleFeatureHandler.Features); 
            Assert.Contains("FeatureC", multipleFeatureHandler.Features);                                             
        }    

        [Fact]
        public void ShouldReturnNullIfHandlerNotExist()
        {
            // Arrange
            var types = new Type[] { };                    
            var provider = new HandlerFeatureInfoProvider(types);

            // Assert
            var handler = provider.GetHandlerInfo("notexisthandler");

            // Assert
            Assert.Null(handler);
        }    
    }
}