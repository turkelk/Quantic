using System.Collections.Generic;
using System;

namespace Contracts
{
    public class SendMail
    {
        public SendMail()
        {
            Params = new Dictionary<int,object>();
        }
        public string To { get; set; }
        public Guid UserGuid { get; set; }
        public string TemplateCode { get; set; }
        public IDictionary<int, object> Params { get; set; }        
    }
}