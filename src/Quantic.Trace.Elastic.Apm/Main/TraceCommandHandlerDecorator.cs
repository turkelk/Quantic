using System.Threading.Tasks;
using Elastic.Apm;
using Elastic.Apm.Api;
using Microsoft.Extensions.Options;
using Quantic.Core;

namespace Quantic.Trace.Elastic.Apm
{
    public class TraceCommandHandlerDecorator<TCommand> : ICommandHandler<TCommand>
             where TCommand : ICommand
    {
        private readonly ICommandHandler<TCommand> decoratedRequestHandler;
        private readonly ITracer tracer;
        private readonly TraceSettings traceSettings;

        public TraceCommandHandlerDecorator(
            ICommandHandler<TCommand> decoratedRequestHandler,
            ITracer tracer,
            IOptionsSnapshot<TraceSettings> traceSettingOptions)
        {
            this.decoratedRequestHandler = decoratedRequestHandler;
            this.tracer = tracer;
            this.traceSettings = traceSettingOptions.Value;
        }

        public async Task<CommandResult> Handle(TCommand command, RequestContext context)
        {
            var commandName = command.GetType().FullName;
            bool shouldCreateSpan = traceSettings?.ShouldCreateSpan(commandName) ?? false;
            
            CommandResult commandResult = null;

            if(!shouldCreateSpan)
                commandResult =  await decoratedRequestHandler.Handle(command, context);                         

            if(!context.Headers.ContainsKey(DistributedTracingHeader.DistributedTracingDataKey))
            {
                string outgoingDistributedTracingData = 
                (tracer.CurrentSpan?.OutgoingDistributedTracingData
                ?? tracer.CurrentTransaction?.OutgoingDistributedTracingData)?.SerializeToString();

                context.Headers.Add(DistributedTracingHeader.DistributedTracingDataKey, outgoingDistributedTracingData); 
            } 

            if(tracer.CurrentTransaction == null)
            {
               await tracer.CaptureTransaction($"{commandName}-Txn", $"{commandName}-Txn", async(transaction) => 
               {
                   // transaction.Labels["TK"] = "kadirzade";
                    await tracer.CurrentTransaction.CaptureSpan(commandName, $"{commandName} Handling", async (span) => 
                    {
                        commandResult =  await decoratedRequestHandler.Handle(command, context);
                    });                   
               }, DistributedTracingData.TryDeserializeFromString(context.Headers[DistributedTracingHeader.DistributedTracingDataKey]));
            }
            else
            {
                await tracer.CurrentTransaction.CaptureSpan(commandName, $"{commandName} Handling", async (span) => 
                {
                    commandResult =  await decoratedRequestHandler.Handle(command, context);
                    span.Labels["result"] = commandResult.FormatResult();  
                });                
            }

            return commandResult;
        }
    }
}