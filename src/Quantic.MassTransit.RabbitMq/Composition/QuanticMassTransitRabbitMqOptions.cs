using System.Reflection;

namespace Quantic.MassTransit.RabbitMq
{
    public class QuanticMassTransitRabbitMqOptions
    {
        public Assembly[] ConsumerAssemblies { get; set; }
        public RabbitMqConfig RabbitConfig { get; set;}
    }
}