using Quantic.Core;

namespace Quantic.Trace.Elastic.Apm
{
    public static class ResultExtension
    {
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