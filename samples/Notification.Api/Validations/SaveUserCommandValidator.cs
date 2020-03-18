using FluentValidation;
using Quantic.Validation;

namespace Notification.Api.Validations
{
    public class SaveUserValidator : QuanticValidator<SaveUserCommand>
    {
        public SaveUserValidator()
        {
            RuleFor(prop => prop.Email)
                .EmailAddress()
                .WithErrorCode(Msg.InvalidEmailAddress);
        }
    }
}