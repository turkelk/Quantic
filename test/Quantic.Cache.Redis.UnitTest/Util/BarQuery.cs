using Quantic.Core;

namespace Quantic.Cache.Redis.UnitTest
{
    public class BarQuery : IQuery<string>
    {
        public string Key {get;set;}
        public string SubKey { get;set;}
    }
}