using System;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Quantic.Core;

namespace Quantic.Web.Test
{
    public class CommandResultExtensionCreatedTest
    {
        [Fact]
        public void ShouldReturnBadRequest()
        {
            // Arrange
            var failure = new ValidationFailure("code", "message");
            var commandResult = new CommandResult(failure);            
            string resourceForLocation = "users";
            
            // Act
            var response = commandResult.Created(resourceForLocation);

            // Assert
            Assert.IsType<BadRequestObjectResult>(response);          
        }

        [Fact]
        public void ShouldReturnInternalServerError()
        {
            // Arrange
            var failure = new Failure("code", "message");
            var commandResult = new CommandResult(failure);            
            string resourceForLocation = "users";
            
            // Act
            var response = commandResult.Created(resourceForLocation);

            // Assert
            Assert.IsType<ObjectResult>(response);         
            Assert.Equal(StatusCodes.Status500InternalServerError, ((ObjectResult)response).StatusCode);            
        }


        [Fact]
        public void ShouldThrowArgumentNullExceptionForEmptyResourceLocation()
        { 
            // Arrange          
            bool exceptionThrown = false;
            string paramName = "resourceForLocation";
            string resourceForLocation = "";
            var commandResult = new CommandResult(Guid.NewGuid());
            
            // Act
            try
            {
               commandResult.Created(resourceForLocation: resourceForLocation);
            }
            catch (ArgumentNullException ex)
            {
                exceptionThrown = true;
                Assert.Equal(paramName, ex.ParamName);
            }

            // Assert
            Assert.True(exceptionThrown);                               
        }   

        [Fact]
        public void ShouldThrowArgumentNullExceptionForNullResourceLocation()
        {           
            // Arrange          
            bool exceptionThrown = false;
            string paramName = "resourceForLocation";
            string resourceForLocation = null;
            var commandResult = new CommandResult(Guid.NewGuid());
            
            // Act
            try
            {
               commandResult.Created(resourceForLocation: resourceForLocation);
            }
            catch (ArgumentNullException ex)
            {
                exceptionThrown = true;
                Assert.Equal(paramName, ex.ParamName);
            }

            // Assert
            Assert.True(exceptionThrown);                                
        } 

        [Fact]
        public void ShouldThrowArgumentExceptionForEmptyGuid()
        {           
            // Arrange
            bool exceptionThrown = false;
            var commandResult = CommandResult.Success;

            try
            {
                commandResult.Created(resourceForLocation : "users");
            }
            catch (Exception)
            {
                exceptionThrown = true;
            }

            Assert.True(exceptionThrown);                               
        }        

        [Fact]
        public void ShouldSuccessWithResourceLocation()
        {   
            // Arrange        
            string resource = "users";
            var guid = Guid.NewGuid();
            var commandResult = new CommandResult(guid);
            string expectedLocation = $"{resource}/{guid}"; 

            // Act
            var response = commandResult.Created(resourceForLocation : resource);

            // Assert
            Assert.IsType<CreatedResult>(response);  
            var result = response as CreatedResult;            
            Assert.Equal(StatusCodes.Status201Created,result.StatusCode);
            Assert.Equal(expectedLocation, result.Location);
            Assert.Null(result.Value);                        
        }                         

        [Fact]
        public void ShouldSuccessWithUri()
        {     
            // Arrange        
            string expectedLocation = "http://foo.com/";
            var uri = new Uri(expectedLocation);
            var commandResult = CommandResult.Success;

            // Act
            var response = commandResult.Created(uri);

            // Assert
            Assert.IsType<CreatedResult>(response);  
            var result = response as CreatedResult;            
            Assert.Equal(StatusCodes.Status201Created,result.StatusCode);
            Assert.Equal(expectedLocation, result.Location);
            Assert.Null(result.Value);            
        }                                
    }
}