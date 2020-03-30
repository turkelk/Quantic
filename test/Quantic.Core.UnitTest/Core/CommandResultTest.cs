using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Quantic.Core.Test
{
    public class CommandResultTest
    {
        [Fact]
        public void ShouldThrowArgumentNullExpcetionForCode()
        {  
            // Assert
            Assert.Throws<ArgumentNullException>(() => 
            {
                // Act
                CommandResult.WithMessage(null, "message");
            });

            Assert.Throws<ArgumentNullException>(() => 
            {
                // Act
                CommandResult.WithMessage("", "message");
            });            
        }

        [Fact]
        public void ShouldThrowArgumentNullExpcetionForFailures()
        {
            // Assert
            Assert.Throws<ArgumentNullException>(() => 
            {
                // Act
                var failureList = new List<Failure>(null);
                var result = CommandResult.WithError(failureList);
            }); 

            // Assert
            Assert.Throws<ArgumentException>(() => 
            {
                // Act
                var failureList = new List<Failure>();
                var result =  CommandResult.WithError(failureList);
            });             
        }

        [Fact]
        public void ShouldThrowArgumentNullExpcetionForGuid()
        {
            // Assert
            Assert.Throws<ArgumentNullException>(() => 
            {
                // Act
                CommandResult.WithGuid(Guid.Empty);
            }); 

            // Assert
            Assert.Throws<ArgumentNullException>(() => 
            {
                // Act
                new CommandResult(Guid.Empty,"code", "message");
            });             
        }

        [Fact]
        public void ShouldSuccessWithGuid()
        {
            // Arrange
            var guid = Guid.NewGuid();

            // Act
            var result = CommandResult.WithGuid(guid);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(guid, result.Guid);
            Assert.Equal(Messages.Success, result.Code);
            Assert.False(result.HasError);
            Assert.False(result.Retry);
        }

                [Fact]
        public void ShouldSuccessWithGuidCodeAndMessage()
        {
            // Arrange
            var guid = Guid.NewGuid();
            string code = "code";
            string message = "message";

            // Act
            var result = new CommandResult(guid,code,message);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(guid, result.Guid);
            Assert.Equal(message, result.Message);
            Assert.Equal(code, result.Code);
            Assert.False(result.HasError);
            Assert.False(result.Retry);
        }

        [Fact]
        public void ShouldSuccessWithstaticSuccessPropery()
        {
            // Act
            var result = CommandResult.Success;

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(Messages.Success, result.Code);
            Assert.False(result.HasError);
            Assert.False(result.Retry);
        }

        [Fact]
        public void ShouldSuccessWithCode()
        {
            // Arrange
            string code = "code";

            // Act
            var result = CommandResult.WithCode(code);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(code, result.Code);
            Assert.False(result.HasError);
            Assert.False(result.Retry);
        }

        [Fact]
        public void ShouldSuccessWithCodeAndNullMessage()
        {
            // Arrange
            string code = "code";
            string message = null;

            // Act
            var result = CommandResult.WithMessage(code, message);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(code, result.Code);
            Assert.Equal(message, result.Message);
            Assert.False(result.HasError);
            Assert.False(result.Retry);
        }

        [Fact]
        public void ShouldSuccessWithCodeAndEmptyMessage()
        {
            // Arrange            
            string code = "code";
            string message = "";

            // Act
            var result = CommandResult.WithMessage(code, message);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(code, result.Code);
            Assert.Equal(message, result.Message);
            Assert.False(result.HasError);
            Assert.False(result.Retry);
        }

        [Fact]
        public void ShouldSuccessWithFailureWithoutRetry()
        {
            // Arrange
            var failireCode = "failure_code";
            var failure = new Failure(failireCode);

            // Act
            var result = CommandResult.WithError(failure);

            // Assert
            Assert.True(result.HasError);
            Assert.False(result.Retry);
            Assert.True(result.Errors.Count == 1
                && result.Errors.Any(x => x.Code == failireCode));
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public void ShouldSuccessWithFailureListWithoutRetry()
        {
            // Arrange
            var failireCode1 = "failure_code_1";
            var failure1 = new Failure(failireCode1);
            var failireCode2 = "failure_code_2";
            var failure2 = new Failure(failireCode2);
            var failures = new List<Failure> { failure1, failure2 };

            // Act
            var result = CommandResult.WithError(failures);

            // Assert
            Assert.True(result.HasError);
            Assert.False(result.Retry);

            Assert.True(result.Errors.Count == 2
                && result.Errors.Any(x => x.Code == failireCode1)
                && result.Errors.Any(x => x.Code == failireCode2));

            Assert.False(result.IsSuccess);
        }

        [Fact]
        public void ShouldSuccessWithFailureAndFalseRetry()
        {
            // Arrange            
            var failireCode = "failure_code";
            var failure = new Failure(failireCode);
            bool retry = false;

            // Act
            var result = new CommandResult(failure, retry: retry);

            // Assert
            Assert.True(result.HasError);
            Assert.False(result.Retry);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public void ShouldSuccessWithFailureAndTrueRetry()
        {
            // Arrange                
            var failireCode = "failure_code";
            var failure = new Failure(failireCode);
            bool retry = true;

            // Act
            var result = new CommandResult(failure, retry: retry);

            // Assert
            Assert.True(result.HasError);
            Assert.True(result.Retry);
            Assert.False(result.IsSuccess);
        }


         [Fact]
        public void ShouldSuccessWithFailureCodeWithoutRetry()
        {
            // Arrange
            var errorCode = "failure_code";

            // Act
            var result = CommandResult.WithError(errorCode);

            // Assert
            Assert.True(result.HasError);
            Assert.False(result.Retry);
            Assert.True(result.Errors.Count == 1
                && result.Errors.Any(x => x.Code == errorCode));
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public void ShouldSuccessWithFailureCodeAndFalseRetry()
        {
            // Arrange            
            var failireCode = "failure_code";
            bool retry = false;

            // Act
            var result = CommandResult.WithError(failireCode, retry: retry);

            // Assert
            Assert.True(result.HasError);
            Assert.False(result.Retry);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public void ShouldSuccessWithFailureCodeAndTrueRetry()
        {
            // Arrange                
            var failireCode = "failure_code";
            bool retry = true;

            // Act
            var result = CommandResult.WithError(failireCode, retry: retry);

            // Assert
            Assert.True(result.HasError);
            Assert.True(result.Retry);
            Assert.False(result.IsSuccess);
        }
    }
}
