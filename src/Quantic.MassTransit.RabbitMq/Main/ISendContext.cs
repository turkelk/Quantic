using System.Collections.Generic;
using GreenPipes;

namespace Quantic.MassTransit.RabbitMq
{
    public interface ISendContext : PipeContext
    {
        Dictionary<string, string> Context { get; }
    }    
}