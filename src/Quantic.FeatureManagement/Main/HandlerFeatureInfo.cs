using System.Collections.Generic;

namespace Quantic.FeatureManagement
{
    public class HandlerFeatureInfo
    {
        public HandlerFeatureInfo(string name, IEnumerable<string> features)
        {
            Name = name;
            Features = features;
        }

        public string Name { get; }
        public IEnumerable<string> Features { get; }
    }    
}