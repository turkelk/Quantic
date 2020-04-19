using System;
using Quantic.Core;

namespace Quantic.FeatureManagement
{
    [AttributeUsage(System.AttributeTargets.Class, AllowMultiple = true)]
    public class DecorateFeatureManagementAttribute : System.Attribute
    {
        public string Feature { get; }
        public DecorateFeatureManagementAttribute(string featureName)
        {
            featureName.Guard(nameof(featureName));
            Feature = featureName;
        }
    }
}