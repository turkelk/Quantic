// using System.Linq;
// using System.Reflection;
// using Newtonsoft.Json;
// using Newtonsoft.Json.Serialization;

// namespace Quantic.Log
// {
//     public class SensitiveDataResolver : DefaultContractResolver
//     {
//         protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
//         {
//             var property = base.CreateProperty(member, memberSerialization);

//             if (member is PropertyInfo prop)
//             {
//                 var customAttribute = prop
//                     .GetCustomAttributes()
//                     .FirstOrDefault(x => typeof(ISensitiveDataAttribute).IsAssignableFrom(x.GetType()));

//                 if (customAttribute != null)
//                 {
//                     property.ValueProvider = ((ISensitiveDataAttribute)customAttribute).ValueProvider;
//                 }
//             }

//             return property;
//         }
//     }
// }