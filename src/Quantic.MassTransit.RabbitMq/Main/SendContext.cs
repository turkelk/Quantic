using System.Collections.Generic;
using GreenPipes;

namespace Quantic.MassTransit.RabbitMq
{
    public class SendContext : BasePipeContext, ISendContext
    {
        public Dictionary<string, string> Context {get;set;}
    }
}