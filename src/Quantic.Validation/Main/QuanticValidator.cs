using FluentValidation;
using Quantic.Core;

namespace Quantic.Validation
{
    public abstract class QuanticValidator<T> : AbstractValidator<T>
    {
        public RequestContext Context { get; internal set;}
    }
}