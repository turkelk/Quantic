using System.Collections.Generic;

namespace Quantic.FeatureManagement
{
    public class Setting
    {
        public string FeatureName {get;set;}
        public bool Enable {get;set;}
        public Dictionary<string, string> Filters = new Dictionary<string, string>();  
    }
}