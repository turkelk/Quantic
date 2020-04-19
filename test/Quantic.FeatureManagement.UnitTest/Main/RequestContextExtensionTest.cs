using System.Collections.Generic;
using Quantic.Core;
using Xunit;

namespace Quantic.FeatureManagement.UnitTest
{
    public class RequestContextExtensionTest
    {       
         [Fact] 
        public void ShouldReturnHeaderValue()
        {
            // Arrange
            string key = "KEY1";
            string value = "VALUE1";
        
            var headers = new Dictionary<string, string>
            {
                { key,value }
            };

            var context = new RequestContext("trace-id", headers);

            // Act - Assert
            Assert.Equal(value, context.GetHeaderValue(key));
        }

        [Fact] 
        public void ShouldReturnNullIfKeyNotExist()
        {
            // Arrange
            var headers = new Dictionary<string, string>();            
            var context = new RequestContext("trace-id", headers);

            // Act - Assert
            Assert.Null(context.GetHeaderValue("KEY1"));
        }                 
    }
}