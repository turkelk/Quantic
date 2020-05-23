using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace Quantic.Web
{
    public class QuanticWebOptions
    {
        public void AddMandatoryHeader(string key)
        {
            MandatoryHeadersHolder.Headers.TryAdd(key, StringOrNullHeaderValidator.Instance);
        } 

        public void AddMandatoryHeader(string key, IHeaderValidator validator)
        {
            MandatoryHeadersHolder.Headers.TryAdd(key, validator);
        }   

        public void AddMandatoryHeadersFromConfig(IConfiguration configuration, string sectionName = "HttpMandatoryHeaders")
        {   
            var headers = configuration.GetSection(sectionName).Get<List<string>>();    
            foreach(var header in headers)
            {
                MandatoryHeadersHolder.Add(header, StringOrNullHeaderValidator.Instance);
            }                                          
        }          
    }
}