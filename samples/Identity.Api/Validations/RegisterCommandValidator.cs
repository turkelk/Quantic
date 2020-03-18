using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Quantic.Core;
using Quantic.Validation;

namespace Identity.Api.Validations
{
    public class RegisterCommandValidator : QuanticValidator<RegisterCommand>
    {
        public RegisterCommandValidator(IQueryHandler<GetUserByEmailQuery, User> getUserByEmailQueryHandler)
        {
            RuleFor(x => x.Name)
            .NotEmpty()
            .WithErrorCode(Msg.NameIsRequiredFieldForUser);

            RuleFor(x => x.LastName)
            .NotEmpty()
            .WithErrorCode(Msg.LastNameIsRequiredFieldForUser);            

            RuleFor(x => x.Email)
            .EmailAddress()
            .WithErrorCode(Msg.InvalidEmailAddress)
            .MustAsync(UserNotExitByEmail())
            .WithErrorCode(Msg.UserAlreadyExistByEmail);

            RuleFor(x => x.Password)
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,15}$")
            .WithErrorCode(Msg.PasswordDoesNotMeetRequirments);

            System.Func<string, CancellationToken, Task<bool>> UserNotExitByEmail()
            {
                return async (mail, cancelationToken) =>
                {
                    var getUser = await getUserByEmailQueryHandler.Handle(new GetUserByEmailQuery { Email = mail }, Context);
                    return getUser.IsSuccess && getUser.Code == Msg.UserNotExistByMail;
                };
            }
        }
    }
}