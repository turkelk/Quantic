namespace Quantic.Cache.Redis.UnitTest
{
    public class CacheKeyProviderWithoutPropperConstructor : ICacheKeyProvider
    {
        public string GetKey()
        {
            return "somekey";
        }
    }
}