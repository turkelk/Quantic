using Quantic.Core;

namespace Quantic.Trace.Elastic.Apm
{
    internal static class Extension
    {        
        public static string ToTransactionName(this string requestName)
        {
            return $"{requestName}-Txn";
        }
        public static string ToTransactionType(this string requestName)
        {
            return $"{requestName}-Txn";
        } 

        public static string ToSpanName(this string requestName)
        {
            return requestName;
        }
        public static string ToSpanType(this string requestName)
        {
            return $"{requestName} Handling";
        } 

        public static string FormatResult(this CommandResult result) 
        {        
            if(result.IsSuccess)
            {
                return $"{result.Code} {result.Message}";
            }    
            else
            {
                return $"{result.Errors[0].Code} {result.Errors[0].Message}";                              
            }
        } 
        public static string FormatResult<TResult>(this QueryResult<TResult> result) 
        {        
            if(result.IsSuccess)
            {
                return $"{result.Code} {result.Message}";
            }    
            else
            {
                return $"{result.Errors[0].Code} {result.Errors[0].Message}";                              
            }
        }                       
    }
}