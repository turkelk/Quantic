namespace Quantic.MassTransit.RabbitMq
{
    public class RabbitMqConfig
    {
        public string Host {get;set;}
        public string UserName {get;set;}
        public string Password {get;set;}
        public string ReceiveEndpoint {get;set;}
        public int Port { get; set; }
    }
}