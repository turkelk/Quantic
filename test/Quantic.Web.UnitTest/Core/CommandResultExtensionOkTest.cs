using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Quantic.Core;

namespace Quantic.Web.Test
{
    public class CommandResultExtensionOkTes
    {

        [Fact]
        public void ShouldReturnBadRequest()
        {
            // Arrange
            var failure = new ValidationFailure("code", "message");
            var commandResult = new CommandResult(failure);            
            
            // Act
            var response = commandResult.Ok();

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
            var response = commandResult.Ok();

            // Assert
            Assert.IsType<ObjectResult>(response);         
            Assert.Equal(StatusCodes.Status500InternalServerError, ((ObjectResult)response).StatusCode);            
        }   

        [Fact]
        public void ShouldCuccess()
        {
            // Arrange
            var commandResult = CommandResult.Success;  

            // Act
            var response = commandResult.Ok();

            // Assert
            Assert.IsType<OkResult>(response);         
            Assert.Equal(StatusCodes.Status200OK, ((OkResult)response).StatusCode);            
        }                        
    }
}