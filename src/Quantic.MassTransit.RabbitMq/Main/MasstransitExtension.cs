using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Quantic.Core;

namespace Quantic.MassTransit.RabbitMq
{
    public static class MasstransitExtension
    {
        private const string quanticTraceId = "quantic-trace-id";

        public async static Task Publish<T>(this IBusControl busControl, 
            T message, 
            RequestContext requestContext) where T : class 
        {
            await busControl.Publish<T>(message, c =>
            {                
                c.SetHeaders(requestContext);
            });   
        }
        public async static Task Publish(this IBusControl busControl, 
            object message, 
            RequestContext requestContext)
        {
            await busControl.Publish(message, c=>
            {                
                c.SetHeaders(requestContext);
            }); 
        }  

        public async static Task Send(this ISendEndpoint endpoint, object message, RequestContext requestContext)
        {
            await endpoint.Send(message, context => 
            {
                context.SetHeaders(requestContext);
            });
        }         

        public async static Task Send<T>(this ISendEndpoint endpoint, T message, RequestContext requestContext) 
            where T:class
        {
            await endpoint.Send<T>(message, context => 
            {
                context.SetHeaders(requestContext);
            });
        }                

        private static void SetHeaders(this SendContext<object> sendContext, RequestContext requestContext)
        {
            foreach(var item in requestContext.Headers)
            {
                sendContext.Headers.Set(item.Key, item.Value);
            }            
        } 

        public static RequestContext RequestContext(this ConsumeContext consumerContext)
        {
            return new RequestContext(consumerContext.Headers.Get<string>(quanticTraceId), 
                consumerContext.Headers.GetAll().ToDictionary(kvp=>  kvp.Key, kvp => kvp.Value.ToString()));
        }                
    }
}