using System;
using Microsoft.AspNetCore.Mvc;
using Quantic.Core;

namespace Quantic.Web
{
    public static class QueryResultExtension
    {
         public static IActionResult Ok<T>(this QueryResult<T> result)
         {     
            return result.GetError() ?? new OkObjectResult(AsQueryResponse(result));          
         }

        public static IActionResult ToResponse<T>(this QueryResult<T> result, Func<QueryResult<T>, IActionResult> func)  
        {  
            return func(result);  
        } 
        private static IActionResult GetError<T>(this QueryResult<T> result)
        {
            return result.HasError
                  ? ResultExtension.Error(result) 
                  : null;
        }
        static object AsQueryResponse<T>(this QueryResult<T> result)
        {
            return new
            {
              Data = result.Result
            };                      
        }   
    }
}