using System.Collections.Generic;

namespace Quantic.Core
{
    public class QueryResult<TResult> : Result
    {
        public TResult Result { get; }

        public static QueryResult<TResult> WithError(string errorCode, bool retry = false)
        {
            return new QueryResult<TResult>(new Failure(errorCode), retry);
        }
        public static QueryResult<TResult> WithError(string errorCode, string errorMessage, bool retry = false)
        {
            return new QueryResult<TResult>(new Failure(errorCode, errorMessage), retry);
        }        
        public static QueryResult<TResult> WithError(Failure error, bool retry = false)
        {
            return new QueryResult<TResult>(error, retry);
        }
        public static QueryResult<TResult> WithError(IList<Failure> errors, bool retry = false)
        {
            return new QueryResult<TResult>(errors, retry);
        }        
        public static QueryResult<TResult> WithResult(TResult result)
        {
            return new QueryResult<TResult>(result);
        }
        
        public static QueryResult<TResult> WithCode(TResult result, string code)
        {
            return new QueryResult<TResult>(result, code);
        }

        public static QueryResult<TResult> WithMessage(TResult result, string code, string message)
        {
            return new QueryResult<TResult>(result, code, message);
        }        
             
        public QueryResult(IList<Failure> errors, bool retry = false)
            : base(errors, retry)
        {
        }

        public QueryResult(Failure error, bool retry = false)
            : base(error, retry)
        {
        }

        public QueryResult(TResult result, string code, string message = "")
            : base(code, message)
        {
            this.Result = result;
        }

        public QueryResult(TResult result)
            : this(result, SuccessMessage)
        {
            this.Result = result;
        }
    }
}
