using System;
using System.Collections.Generic;
using System.Linq;

namespace Quantic.Core
{
    public class Result
    {  
        public Result(IList<Failure> errors, bool retry = false)
        {
            if(errors is null)
                throw new ArgumentNullException(nameof(errors));

            if(!errors.Any())
                throw new ArgumentException("Error list is empty",nameof(errors));


            Errors = errors;
            Retry = retry;
        }

        public Result(Failure error, bool retry = false)
             : this(new List<Failure> { error }, retry)
        {

        }

        public Result(string code, string message = default)
        {
            Code = string.IsNullOrEmpty(code)
                ? throw new ArgumentNullException(nameof(code))
                :code;

            Message = message;

            Errors = new List<Failure>();
        }

        public string Code { get; }
        public string Message { get; }
        public IList<Failure> Errors { get; }
        public bool Retry { get; }

        public bool IsSuccess
        {
            get
            {
                return Errors is null || Errors.Count == 0;
            }
        }

        public bool HasError
        {
            get
            {
                return !IsSuccess;
            }
        }

        protected static readonly string SuccessMessage = Messages.Success;

        public string ErrorsToString()
        {
            if (IsSuccess)
                return null;

            return $"Retry:{Retry}, Errors:{string.Join(System.Environment.NewLine, Errors.Select(x => x.ToString()))}";
        }     
    }
}
