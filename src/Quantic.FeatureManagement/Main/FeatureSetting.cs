using System.Collections.Generic;

namespace Quantic.FeatureManagement
{
    public class FeatureSetting
    {
        public string FeatureName {get;set;}
        public bool Enable {get;set;}
        public Dictionary<string, string> Filters = new Dictionary<string, string>();  
    }
}