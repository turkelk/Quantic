namespace Quantic.Cache.Redis
{
    public interface ICacheKeyProvider
    {
        string GetKey();        
    }
}