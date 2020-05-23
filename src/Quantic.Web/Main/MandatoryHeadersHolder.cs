using System.Collections.Generic;

namespace Quantic.Web
{
    public static class MandatoryHeadersHolder
    {
        internal static readonly Dictionary<string, IHeaderValidator> Headers = new Dictionary<string, IHeaderValidator>();

        internal static bool Add(string key, IHeaderValidator validator)
        {
            return Headers.TryAdd(key, validator);
        }
        
        public static void Validate(Dictionary<string, string> httpRequestHeaders)
        {            
            foreach(var mandatoryHeader in Headers)
            {
                if(!httpRequestHeaders.TryGetValue(mandatoryHeader.Key, out string value))
                {
                    throw new HeaderMissingException($"Mandatory header {mandatoryHeader.Key} is missing in http headers.");                    
                } 

                var validationResult =  Headers[mandatoryHeader.Key].Validate(mandatoryHeader.Key, value);

                if(!validationResult.IsSuccess)
                {  
                    throw new System.Exception($"Header validation failed for header {mandatoryHeader.Key} for value {value ?? "null"} with message {validationResult.Code}:{validationResult.Message} ");                                      
                }     
            }
        }        
    }
}