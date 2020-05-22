using System.Threading.Tasks;
using Elastic.Apm;
using Elastic.Apm.Api;
using Microsoft.Extensions.Options;
using Quantic.Core;

namespace Quantic.Trace.Elastic.Apm
{
    public class TraceQueryHandlerDecorator<TQuery, TResponse> : IQueryHandler<TQuery, TResponse>
                   where TQuery : IQuery<TResponse>
    {
        private readonly IQueryHandler<TQuery, TResponse> decoratedRequestHandler;
        private readonly TraceSettings traceSettings;
        public TraceQueryHandlerDecorator( IQueryHandler<TQuery, TResponse> decoratedRequestHandler,
            IOptionsSnapshot<TraceSettings> traceSettingOptions)
        {
            this.decoratedRequestHandler = decoratedRequestHandler;
            this.traceSettings = traceSettingOptions.Value;
        }

        public async Task<QueryResult<TResponse>> Handle(TQuery query, RequestContext context)
        {
            var queryName = query.GetType().FullName;
            bool shouldCreateSpan = traceSettings?.ShouldCreateSpan(queryName) ?? false;
            QueryResult<TResponse> queryResult = null; 

            if(!shouldCreateSpan)
                queryResult =  await decoratedRequestHandler.Handle(query, context);
                        
            if(!context.Headers.ContainsKey(DistributedTracingHeader.DistributedTracingDataKey))
            {
                string outgoingDistributedTracingData = 
                (Agent.Tracer.CurrentSpan?.OutgoingDistributedTracingData
                ?? Agent.Tracer.CurrentTransaction?.OutgoingDistributedTracingData)?.SerializeToString();

                context.Headers.Add(DistributedTracingHeader.DistributedTracingDataKey, outgoingDistributedTracingData); 
            } 

            if(Agent.Tracer.CurrentTransaction == null)
            {
               await Agent.Tracer.CaptureTransaction($"{queryName}-Transaction", $"{queryName}-Transaction", async() => 
               {
                    await Agent.Tracer.CurrentTransaction.CaptureSpan(queryName, $"{queryName} Handling", async (span) => 
                    {
                        queryResult =  await decoratedRequestHandler.Handle(query, context);
                    });                   
               }, DistributedTracingData.TryDeserializeFromString(context.Headers[DistributedTracingHeader.DistributedTracingDataKey]));
            }
            else
            {
                await Agent.Tracer.CurrentTransaction.CaptureSpan(queryName, $"{queryName} Handling", async (span) => 
                {
                    queryResult =  await decoratedRequestHandler.Handle(query, context);
                });                
            }  

            return queryResult;        
        }
    }
}