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

        public void AddMandatoryHeaders(List<string> keys)
        {   
            foreach(var key in keys)
            {
                MandatoryHeadersHolder.Add(key, StringOrNullHeaderValidator.Instance);
            }                                          
        }          
    }
}