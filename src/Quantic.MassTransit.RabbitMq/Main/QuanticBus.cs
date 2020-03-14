using System.Threading.Tasks;
using MassTransit;
using Quantic.Core;
using Quantic.Core.Util;

namespace Quantic.MassTransit.RabbitMq
{
    public class QuanticBus : IQuanticBus
    {
        private readonly IBusControl busControl;

        public QuanticBus(IBusControl busControl)
        {
            busControl.Guard(nameof(busControl));
            this.busControl = busControl;
        }

        public async Task Publish<T>(T message, RequestContext context) where T : class
        {
            await busControl.Publish<T>(message, context);
        }
    }
}