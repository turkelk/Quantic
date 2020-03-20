using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Quantic.Core;
using Quantic.Cache.InMemory;

namespace Notification.Api
{
    [DecorateInMemoryCache]
    public class GetMailTemplateByTemplateCodeHandler : IQueryHandler<GetMailTemplateByTemplateCode, MailTemplate>
    {
        private readonly DataContext dataContext;

        public GetMailTemplateByTemplateCodeHandler(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public async Task<QueryResult<MailTemplate>> Handle(GetMailTemplateByTemplateCode query, RequestContext context)
        {
            var template = await dataContext.MailTemplates.FirstOrDefaultAsync(t => t.Code == query.TemplateCode);

            if (template == null)
            {
                return new QueryResult<MailTemplate>(null, Messages.NotFound);
            }

            return new QueryResult<MailTemplate>(template);
        }
    }
    public class GetMailTemplateByTemplateCode : IQuery<MailTemplate>
    {
        public GetMailTemplateByTemplateCode(string templateCode)
        {
            this.TemplateCode = templateCode;
        }
        public string TemplateCode { get; set; }
    }
}