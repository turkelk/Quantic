using System;
using System.Collections.Generic;
using Quantic.Core;

namespace Notification.Api
{
    public class SendMailCommand: ICommand
    {
        public string To { get; set; }
        public Guid UserGuid { get; set; }
        public string TemplateCode { get; set; }
        public IDictionary<string, string> Params { get; set; }
    }
}