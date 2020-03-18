using System;
using Xunit;

namespace Quantic.Core.Test
{
    public class FailureTest
    {
        [Fact]
        public void ShouldThrowArgumentNullExpcetionForCode()
        {
            // Assert
            Assert.Throws<ArgumentNullException>(() => 
            {
                // Act
                new Failure(null, "message");
            });

            Assert.Throws<ArgumentNullException>(() => 
            {
                // Act
                new Failure("", "message");
            }); 
        }

        [Fact]
        public void ShouldSuccessWithCode()
        {
            // Arrange
            string code = "code";

            // Act
            var failure = new Failure(code);

            // Assert
            Assert.Equal(code, failure.Code);
            Assert.Equal(default, failure.Message);
        }

        [Fact]
        public void ShouldSuccessWithCodeAndNullMessage()
        {
            // Arrange
            string code = "code";
            string message = null;

            // Act
            var failure = new Failure(code, message);

            // Assert
            Assert.Equal(code, failure.Code);
            Assert.Equal(message, failure.Message);
        }

        [Fact]
        public void ShouldSuccessWithCodeAndEmptyMessage()
        {
            // Arrange
            string code = "code";
            string message = "";

            // Act
            var failure = new Failure(code, message);

            // Assert
            Assert.Equal(code, failure.Code);
            Assert.Equal(message, failure.Message);
        }
    }
}
