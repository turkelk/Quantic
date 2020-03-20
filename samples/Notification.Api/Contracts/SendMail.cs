using System.Collections.Generic;
using System;

namespace Contracts
{
    public class SendMail
    {
        public string To { get; set; }
        public Guid UserGuid { get; set; }
        public string TemplateCode { get; set; }
        public IDictionary<string, string> Params { get; set; }        
    }
}