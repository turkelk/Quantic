using Newtonsoft.Json;

namespace Quantic.Log
{
    internal static class SerializerSettings
    {
        public static JsonSerializerSettings Value = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            ContractResolver = new SensitiveDataResolver()
        };  
    }
}