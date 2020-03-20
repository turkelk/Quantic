using System;
using FluentValidation;
using Quantic.Validation;

namespace Notification.Api
{
    public class SendMailCommandValidator: QuanticValidator<SendMailCommand>
    {
        public SendMailCommandValidator()
        {
            RuleFor(p => p.To)
            .EmailAddress()
            .When(p => p.UserGuid == Guid.Empty)
            .WithErrorCode(Msg.InvalidEmailAddress);

            RuleFor(prop => prop.TemplateCode)
            .NotEmpty()
            .WithErrorCode(Msg.TemplateCodeIsRequired);                                    
        }
    }
}