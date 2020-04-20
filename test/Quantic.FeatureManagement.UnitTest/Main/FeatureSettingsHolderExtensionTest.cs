// using System.Collections.Generic;
// using Quantic.Core;
// using Xunit;

// namespace Quantic.FeatureManagement.UnitTest
// {
//     public class FeatureSettingsHolderExtensionTest
//     {
//         [Fact]
//         public void ShouldSuccessWithoutFilter()
//         {
//             // Arrange
//             var feature1 = new FeatureSetting();
//             feature1.Name = "Feature1";
//             feature1.Enable = true;

//             var feature2 = new FeatureSetting();
//             feature2.Name = "Feature2";
//             feature2.Enable = true;    

//             var feature3 = new FeatureSetting();
//             feature3.Name = "Feature3";
//             feature3.Enable = false;     

//             var settings = new FeatureSetting[] 
//             {
//                 feature1,
//                 feature2,
//                 feature3
//             };

//             var holder = new FeatureSettingsHolder();
//             holder.Settings = settings;
//             var emptyHeaders = new Dictionary<string,string>();
//             var context = new RequestContext("trace-id",emptyHeaders);

//             // Act - Assert
//             Assert.True(holder.FatureEnabled("Feature1", context)); 
//             Assert.True(holder.FatureEnabled("Feature2", context)); 
//             Assert.False(holder.FatureEnabled("Feature3", context));      
//             Assert.False(holder.FatureEnabled("Feature4", context));                                            
//         }

//         [Fact]
//         public void ShouldSuccessWithFilter()
//         {
//             // Arrange
//             var feature1 = new FeatureSetting();
//             feature1.Name = "Feature1";
//             feature1.Enable = true;
//             feature1.Filters = new Dictionary<string, string>
//             {
//                 {"KEY1","VALUE1"}
//             };            

//             var feature2 = new FeatureSetting();
//             feature2.Name = "Feature2";
//             feature2.Enable = true;  
//             feature2.Filters = new Dictionary<string, string>
//             {
//                 {"KEY2","VALUE2"}
//             };

//             var settings = new FeatureSetting[] 
//             {
//                 feature1,
//                 feature2
//             };

//             var holder = new FeatureSettingsHolder();
//             holder.Settings = settings;
//             var headers = new Dictionary<string,string>
//             {
//                 {"KEY1","VALUE1"}
//             };
//             var context = new RequestContext("trace-id", headers);

//             // Act - Assert
//             Assert.True(holder.FatureEnabled("Feature1", context)); 
//             Assert.False(holder.FatureEnabled("Feature2", context)); 
//             Assert.False(holder.FatureEnabled("Feature3", context));                                               
//         }        
//     }
// }