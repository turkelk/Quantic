using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Quantic.Core;

namespace Quantic.Validation
{
    public class ValidationQueryHandlerDecorator<TQuery, TResponse> : IQueryHandler<TQuery, TResponse>
           where TQuery : IQuery<TResponse>
           {
        private readonly IQueryHandler<TQuery, TResponse> decoratedRequestHandler;
        private readonly QuanticValidator<TQuery> validator;

        public ValidationQueryHandlerDecorator(IQueryHandler<TQuery, TResponse> decoratedRequestHandler,
            QuanticValidator<TQuery> validator)
        {
            this.decoratedRequestHandler = decoratedRequestHandler;
            this.validator = validator;
        }

        public async Task<QueryResult<TResponse>> Handle(TQuery query, RequestContext context)
        {
            validator.Context = context;
            
            var validationResult = await validator.ValidateAsync(query);

            return validationResult.IsValid
                ? await decoratedRequestHandler.Handle(query, context)
                : new QueryResult<TResponse>(validationResult
                    .Errors.Select(x => new ValidationFailure(x.ErrorCode, x.ErrorMessage))
                    .ToArray());
        }
    }
}
