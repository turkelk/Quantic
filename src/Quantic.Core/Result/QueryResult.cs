using System.Collections.Generic;

namespace Quantic.Core
{
    public class QueryResult<TResult> : Result
    {
        public TResult Result { get; }

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
