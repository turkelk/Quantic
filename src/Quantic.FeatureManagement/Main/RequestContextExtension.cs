using Quantic.Core;

namespace Quantic.FeatureManagement
{
    public static class RequestContextExtension
    {
        public static string GetHeaderValue(this RequestContext context, string key)
        {            
            context.Headers.TryGetValue(key, out string value);
            return value;
        }        
    }
}