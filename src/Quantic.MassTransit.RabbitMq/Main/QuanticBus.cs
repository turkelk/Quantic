using System.Threading.Tasks;
using MassTransit;
using Quantic.Core;
using Quantic.Core.Util;

namespace Quantic.MassTransit.RabbitMq
{
    public class QuanticBus : IQuanticBus
    {
        private readonly IBusControl busControl;
        private readonly ISendEndpointProvider sendEndpointProvider;
        public QuanticBus(IBusControl busControl, ISendEndpointProvider sendEndpointProvider)
        {
            busControl.Guard(nameof(busControl));
            busControl.Guard(nameof(sendEndpointProvider));

            this.busControl = busControl;
            this.sendEndpointProvider = sendEndpointProvider;
        }

        public async Task Publish<T>(T message, RequestContext context) where T : class
        {
            await busControl.Publish<T>(message, context);
        }
        public async Task Publish(object message, RequestContext context)
        {
            await busControl.Publish(message, context);
        }

        public async Task Send(string queue, object message, RequestContext context)
        {
            var sendEndpoint = await sendEndpointProvider.GetSendEndpoint(new System.Uri($"queue:{queue}"));
            await sendEndpoint.Send(message, context);
        } 
        public async Task Send<T>(string queue, T message, RequestContext context) where T : class
        {
            var sendEndpoint = await sendEndpointProvider.GetSendEndpoint(new System.Uri($"queue:{queue}"));
            await sendEndpoint.Send<T>(message, context);
        }     
                          
        private static void SetHeaders(PublishContext publishContext)
        {
            foreach(var item in publishContext.Headers)
            {
                publishContext.Headers.Set(item.Key, item.Value);
            }            
        }                     
    }
}