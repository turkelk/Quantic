using System.Linq;
using Xunit;

namespace Quantic.FeatureManagement.UnitTest
{
    public class HandlerInfoTest
    {
        [Fact] 
        public void ShouldSuccess()
        {
            // Arrange
            var requestName = "Handler";
            string[] features = null;

            // Act
            var handlerInfo = new HandlerFeatureInfo(requestName, null);

            // Assert
            Assert.Equal(requestName, handlerInfo.Name);
            Assert.Equal(features, handlerInfo.Features);


            // Arrange
            var featureName = "Feature";
            features = new string[]
            {
                featureName
            };  

            // Act
            handlerInfo = new HandlerFeatureInfo(requestName, features);  

            // Assert
            Assert.NotEmpty(handlerInfo.Features);
            Assert.Contains(featureName, handlerInfo.Features);          
        }
    }
}