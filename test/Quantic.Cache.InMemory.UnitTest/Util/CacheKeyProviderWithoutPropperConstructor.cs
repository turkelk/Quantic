namespace Quantic.Cache.InMemory.UnitTest
{
    public class CacheKeyProviderWithoutPropperConstructor : ICacheKeyProvider
    {
        public string GetKey()
        {
            return "somekey";
        }
    }
}