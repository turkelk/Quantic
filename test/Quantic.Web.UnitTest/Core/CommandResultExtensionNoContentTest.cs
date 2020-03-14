using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Quantic.Core;

namespace Quantic.Web.Test
{
    public class CommandResultExtensionNoContentTest
    {
        int expectedStatusCode = StatusCodes.Status204NoContent; 

        [Fact]
        public void ShouldReturnBadRequest()
        {
            // Arrange
            var failure = new ValidationFailure("code", "message");
            var commandResult = new CommandResult(failure);            
            
            // Act
            var response = commandResult.NoContent();

            // Assert
            Assert.IsType<BadRequestObjectResult>(response);          
        }


        [Fact]
        public void ShouldReturnInternalServerError()
        {
            // Arrange
            var failure = new Failure("code", "message");
            var commandResult = new CommandResult(failure);            
            
            // Act
            var response = commandResult.NoContent();

            // Assert
            Assert.IsType<ObjectResult>(response);         
            Assert.Equal(StatusCodes.Status500InternalServerError, ((ObjectResult)response).StatusCode);            
        }   

        [Fact]
        public void ShouldSuccess()
        {
            // Arrange
            var commandResult = CommandResult.Success;  

            // Act
            var response = commandResult.NoContent();

            // Assert
            Assert.IsType<NoContentResult>(response);         
            Assert.Equal(StatusCodes.Status204NoContent, ((NoContentResult)response).StatusCode);                  
        }                        
    }
}