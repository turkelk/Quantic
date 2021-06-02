using Quantic.Core;

namespace Quantic.Cache.InMemory
{
    public interface ICacheKeyProvider
    {
        string GetKey(object query, RequestContext context);        
    }
}