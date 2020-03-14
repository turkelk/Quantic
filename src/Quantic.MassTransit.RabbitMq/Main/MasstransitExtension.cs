using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Quantic.Core;

namespace Quantic.MassTransit.RabbitMq
{
    public static class MasstransitExtension
    {
        public async static Task Publish<T>(this IBusControl busControl, 
            T message, 
            RequestContext context) where T : class 
        {
            await busControl.Publish<T>(message, c=>
            {                
                foreach(var item in context.Headers)
                {
                    c.Headers.Set(item.Key, item.Value);
                }
            });   
        }
        public static RequestContext RequestContext(this ConsumeContext consumerContext)
        {
            return new RequestContext(consumerContext.Headers.Get<string>("trace-id"), 
                consumerContext.Headers.GetAll().ToDictionary(kvp=>  kvp.Key, kvp => kvp.Value.ToString()));
        }                
    }
}