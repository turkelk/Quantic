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
        private readonly ITracer tracer;
        private readonly TraceSettings traceSettings;
        public TraceQueryHandlerDecorator( IQueryHandler<TQuery, TResponse> decoratedRequestHandler,
            ITracer tracer,
            IOptionsSnapshot<TraceSettings> traceSettingOptions)
        {
            this.decoratedRequestHandler = decoratedRequestHandler;
            this.tracer = tracer;
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
                (tracer.CurrentSpan?.OutgoingDistributedTracingData
                ?? tracer.CurrentTransaction?.OutgoingDistributedTracingData)?.SerializeToString();

                context.Headers.Add(DistributedTracingHeader.DistributedTracingDataKey, outgoingDistributedTracingData); 
            } 

            if(tracer.CurrentTransaction == null)
            {
               await tracer.CaptureTransaction($"{queryName}-Transaction", $"{queryName}-Transaction", async() => 
               {
                    await tracer.CurrentTransaction.CaptureSpan(queryName, $"{queryName} Handling", async (span) => 
                    {
                        queryResult =  await decoratedRequestHandler.Handle(query, context);
                    });                   
               }, DistributedTracingData.TryDeserializeFromString(context.Headers[DistributedTracingHeader.DistributedTracingDataKey]));
            }
            else
            {
                await tracer.CurrentTransaction.CaptureSpan(queryName, $"{queryName} Handling", async (span) => 
                {
                    queryResult =  await decoratedRequestHandler.Handle(query, context);
                    span.Labels["result"] = queryResult.FormatResult(); 
                    span.Type  = "";                
                });                
            }  

            return queryResult;        
        }
    }
}