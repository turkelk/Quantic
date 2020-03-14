using System;
using Xunit;

namespace Quantic.Core.Test
{
    public class ValidationFailureTest
    {
        [Fact]
        public void Should_throw_argument_null_exception_for_null_code()
        {
            bool exceptionThrown = false;
            string paramName = "code";

            try
            {
                string code = null;
                string message = "message";

                var failure = new ValidationFailure(code, message);
            }
            catch (ArgumentNullException ex)
            {
                exceptionThrown = true;
                Assert.Equal(paramName, ex.ParamName);
            }

            Assert.True(exceptionThrown);
        }

        [Fact]
        public void Should_throw_argument_null_exception_for_empty_code()
        {
            bool exceptionThrown = false;
            string paramName = "code";

            try
            {
                string code = "";
                string message = "message";

                var failure = new ValidationFailure(code, message);
            }
            catch (ArgumentNullException ex)
            {
                exceptionThrown = true;
                Assert.Equal(paramName, ex.ParamName);
            }

            Assert.True(exceptionThrown);
        }

        [Fact]
        public void Should_success_with_code()
        {
            string code = "code";
            var failure = new ValidationFailure(code);
            Assert.Equal(code, failure.Code);
            Assert.Equal(default, failure.Message);
        }

        [Fact]
        public void Should_success_with_code_and_null_message()
        {
            string code = "code";
            string message = null;

            var failure = new ValidationFailure(code, message);
            Assert.Equal(code, failure.Code);
            Assert.Equal(message, failure.Message);
        }

        [Fact]
        public void Should_success_with_code_and_empty_message()
        {
            string code = "code";
            string message = "";

            var failure = new ValidationFailure(code, message);
            Assert.Equal(code, failure.Code);
            Assert.Equal(message, failure.Message);
        }
    }
}
