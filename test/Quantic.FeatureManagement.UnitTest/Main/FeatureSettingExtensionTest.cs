using System.Collections.Generic;
using Quantic.Core;
using Xunit;

namespace Quantic.FeatureManagement.UnitTest
{
    public class FeatureSettingExtensionTest
    {
        [Fact]
        public void ShouldSuccessWithoutFilter()
        {
            // Arrange
            var feature1 = new FeatureSetting();
            feature1.FeatureName = "Feature1";
            feature1.Enable = true;

            var feature2 = new FeatureSetting();
            feature2.FeatureName = "Feature2";
            feature2.Enable = true;    

            var feature3 = new FeatureSetting();
            feature3.FeatureName = "Feature3";
            feature3.Enable = false;     

            var emptyHeaders = new Dictionary<string,string>();
            var context = new RequestContext("trace-id",emptyHeaders);

            // Act - Assert
            Assert.True(feature1.Enabled(context)); 
            Assert.True(feature2.Enabled(context)); 
            Assert.False(feature3.Enabled(context));      
            Assert.False(default(FeatureSetting).Enabled(context));                                            
        }

        [Fact]
        public void ShouldSuccessWithFilter()
        {
            // Arrange
            var feature1 = new FeatureSetting();
            feature1.FeatureName = "Feature1";
            feature1.Enable = true;
            feature1.Filters = new Dictionary<string, string>
            {
                {"KEY1","VALUE1"}
            };            

            var feature2 = new FeatureSetting();
            feature2.FeatureName = "Feature2";
            feature2.Enable = true;  
            feature2.Filters = new Dictionary<string, string>
            {
                {"KEY2","VALUE2"}
            };

            var headers = new Dictionary<string,string>
            {
                {"KEY1","VALUE1"}
            };
            var context = new RequestContext("trace-id", headers);

            // Act - Assert
            Assert.True(feature1.Enabled(context)); 
            Assert.False(feature2.Enabled(context)); 
            Assert.False(default(FeatureSetting).Enabled(context));                                               
        }         
    }
}