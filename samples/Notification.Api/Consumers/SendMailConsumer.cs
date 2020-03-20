using System.Threading.Tasks;
using Contracts;
using MassTransit;
using Quantic.Core;
using Quantic.MassTransit.RabbitMq;

namespace Notification.Api.Consumers
{
    public class SendMailConsumer : IConsumer<SendMail>
    {
        private readonly ICommandHandler<SendMailCommand> sendMailCommandHandler;
        
        public SendMailConsumer(ICommandHandler<SendMailCommand> sendMailCommandHandler)
        {            
            this.sendMailCommandHandler = sendMailCommandHandler;
        }

        public async Task Consume(ConsumeContext<SendMail> context)
        {
            await sendMailCommandHandler.Handle(new SendMailCommand
            {
                To = context.Message.To,
                UserGuid = context.Message.UserGuid,
                TemplateCode = context.Message.TemplateCode,
                Params = context.Message.Params
            }, context.RequestContext());
        }
    }
}