using System.Threading.Tasks;
using Quantic.Core;

namespace Quantic.MassTransit.RabbitMq
{
    public interface IQuanticBus
    {
        Task Publish<T>(T message, RequestContext context) where T : class;
    }
}