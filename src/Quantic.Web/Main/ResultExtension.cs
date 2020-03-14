using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Quantic.Core;

namespace Quantic.Web
{
    public static class ResultExtension
    {
        internal static IActionResult Error(this Result result)
        {
            if(result.IsSuccess)
                throw new System.Exception("Result is not error. Check code");
               
              if(result.HasValidationFaiulure())
              {
                return new BadRequestObjectResult(result.Errors.AsResponseBody());
              }
              else
              {
                var response = new ObjectResult(result.Errors.AsResponseBody());
                response.StatusCode = StatusCodes.Status500InternalServerError;
                return response;                    
              }                        
        }

        static bool HasValidationFaiulure(this Result result)
        {
            return result.Errors.Any(error => error is ValidationFailure);
        } 

        internal static object AsResponseBody(this IList<Failure> errors)
        {
            return new
            {
                     Errors = errors.Select( err=> 
                        new { 
                            err.Code, 
                            err.Message 
                        }
                    )
            };
        }
    }
}