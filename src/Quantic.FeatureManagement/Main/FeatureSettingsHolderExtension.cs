using System.Linq;
using Quantic.Core;

namespace Quantic.FeatureManagement
{
    public static class FeatureSettingsHolderExtension
    {
        public static bool Enabled(this FeatureSetting featureSettings, RequestContext context)
        {
            if(featureSettings == null) 
                return false;
                
            if (!featureSettings.Filters.Any())
                return featureSettings.Enable;

            bool filterMatch = true;

            foreach (var filter in featureSettings.Filters)
            {
                var headerValue = context.GetHeaderValue(filter.Key);

                if (headerValue != filter.Value)
                {
                    filterMatch = false;
                }
            }

            return filterMatch;
        }        
    }
}