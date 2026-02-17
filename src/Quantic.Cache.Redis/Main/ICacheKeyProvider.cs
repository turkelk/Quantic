using Quantic.Core;

namespace Quantic.Cache.Redis
{
    public interface ICacheKeyProvider
    {
        string GetKey(object query, RequestContext context);
    }
}
