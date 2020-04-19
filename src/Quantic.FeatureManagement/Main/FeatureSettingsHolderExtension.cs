using System.Linq;
using Quantic.Core;

namespace Quantic.FeatureManagement
{
    public static class FeatureSettingsHolderExtension
    {
        public static bool FatureEnabled(this FeatureSettingsHolder featureSettingsHolder, string featureName, RequestContext context)
        {
            var feature = featureSettingsHolder.Settings.FirstOrDefault(x => x.Name == featureName);

            if (feature == null || !feature.Enable)
                return false;

            if (!feature.Filters.Any())
                return true;

            bool filterMatch = true;

            foreach (var filter in feature.Filters)
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