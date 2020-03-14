using System;
using Microsoft.AspNetCore.Mvc;
using Quantic.Core;

namespace Quantic.Web
{
    public static class CommandResultExtension
    {
        public static IActionResult Accepted(this CommandResult result)
        {
            return result.GetError() ?? new AcceptedResult();         
        } 

        public static IActionResult Accepted(this CommandResult result, Uri uri)
        {
            return result.GetError() ?? new AcceptedResult(uri, SuccessResponseBody);          
        } 

        public static IActionResult Accepted(this CommandResult result, string resourceForLocation)
        {
            if(string.IsNullOrEmpty(resourceForLocation))
                throw new ArgumentNullException(nameof(resourceForLocation));

            if(result.IsSuccess && result.Guid == Guid.Empty)
                throw new Exception("Guid is empty");

            return result.GetError() ?? new AcceptedResult(result.Location(resourceForLocation), SuccessResponseBody);        
        }                     

        public static IActionResult Created(this CommandResult result, Uri uri)
        {
            return result.GetError() ?? new CreatedResult(uri, SuccessResponseBody);        
        }

        public static IActionResult Created(this CommandResult result, string resourceForLocation)
        {
            if (string.IsNullOrEmpty(resourceForLocation))
                throw new ArgumentNullException(nameof(resourceForLocation));

            if (result.IsSuccess && result.Guid == Guid.Empty)
                throw new Exception("Guid is empty");

            return result.GetError() ?? new CreatedResult(result.Location(resourceForLocation), SuccessResponseBody);
        }

        private static string Location(this CommandResult result, string resourceForLocation)
        {
            return $"{resourceForLocation}/{result.Guid}";
        }

        public static IActionResult NoContent(this CommandResult result)
        {
            return result.GetError() ?? new NoContentResult();          
        }                

        public static IActionResult Ok(this CommandResult result)
        {
            return result.GetError() ?? new OkResult();          
        }  

        public static IActionResult ToResponse(this CommandResult result, Func<CommandResult, IActionResult> func)  
        {  
            return func(result);  
        }         
        private static IActionResult GetError(this CommandResult result)
        {
            return result.HasError
                  ? ResultExtension.Error(result) 
                  : null;
        }

        private static object SuccessResponseBody = null;           
    }
}