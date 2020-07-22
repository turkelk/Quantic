using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Quantic.Core.Test
{
    public class ResultTest
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

                var result = new Result(code, message);
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

                var result = new Result(code, message);
            }
            catch (ArgumentNullException ex)
            {
                exceptionThrown = true;
                Assert.Equal(paramName, ex.ParamName);
            }

            Assert.True(exceptionThrown);
        }

        [Fact]
        public void Should_throw_argument_null_exception_for_null_failure_list()
        {
            bool exceptionThrown = false;

            try
            {
                var result = new Result(errors: null);
            }
            catch (ArgumentNullException)
            {
                exceptionThrown = true;
            }

            Assert.True(exceptionThrown);
        }

        [Fact]
        public void Should_throw_argument_null_exception_for_null_in_failure_list()
        {
            bool exceptionThrown = false;

            try
            {
                var failureList = new List<Failure>(null);
                var result = new Result(failureList);
            }
            catch (ArgumentNullException)
            {
                exceptionThrown = true;
            }

            Assert.True(exceptionThrown);
        }

        [Fact]
        public void Should_throw_argument_null_exception_for_empty_failure_list()
        {
            bool exceptionThrown = false;
            string paramName = "errors";

            try
            {
                var failureList = new List<Failure>();
                var result = new Result(failureList);
            }
            catch (ArgumentException ex)
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
            var result = new Result(code);

            Assert.True(result.IsSuccess);
            Assert.Equal(code, result.Code);

            Assert.False(result.HasError);
            Assert.False(result.Retry);
        }

        [Fact]
        public void Should_success_with_code_and_null_message()
        {
            string code = "code";
            string message = null;
            var result = new Result(code, message);

            Assert.True(result.IsSuccess);
            Assert.Equal(code, result.Code);
            Assert.Equal(message, result.Message);

            Assert.False(result.HasError);
            Assert.False(result.Retry);
        }

        [Fact]
        public void Should_success_with_code_and_empty_message()
        {
            string code = "code";
            string message = "";
            var result = new Result(code, message);

            Assert.True(result.IsSuccess);
            Assert.Equal(code, result.Code);
            Assert.Equal(message, result.Message);

            Assert.False(result.HasError);
            Assert.False(result.Retry);
        }

        [Fact]
        public void Should_success_with_failure_without_retry()
        {
            var failireCode = "failure_code";
            var failure = new Failure(failireCode);

            var result = new Result(failure);

            Assert.True(result.HasError);
            Assert.False(result.Retry);
            Assert.True(result.Errors.Count == 1
                && result.Errors.Any(x => x.Code == failireCode));

            Assert.False(result.IsSuccess);
        }

        [Fact]
        public void Should_success_with_failure_list_without_retry()
        {
            var failireCode1 = "failure_code_1";
            var failure1 = new Failure(failireCode1);

            var failireCode2 = "failure_code_2";
            var failure2 = new Failure(failireCode2);

            var failures = new List<Failure> { failure1, failure2 };

            var result = new Result(failures);

            Assert.True(result.HasError);
            Assert.False(result.Retry);

            Assert.True(result.Errors.Count == 2
                && result.Errors.Any(x => x.Code == failireCode1)
                && result.Errors.Any(x=>x.Code == failireCode2));

            Assert.False(result.IsSuccess);
        }

        [Fact]
        public void Should_success_with_failure_and_false_retry()
        {
            var failireCode = "failure_code";
            var failure = new Failure(failireCode);
            bool retry = false;

            var result = new Result(failure, retry: retry);

            Assert.True(result.HasError);
            Assert.False(result.Retry);

            Assert.False(result.IsSuccess);
        }

        [Fact]
        public void Should_success_with_failure_and_true_retry()
        {
            var failireCode = "failure_code";
            var failure = new Failure(failireCode);
            bool retry = true;

            var result = new Result(failure, retry: retry);

            Assert.True(result.HasError);
            Assert.True(result.Retry);

            Assert.False(result.IsSuccess);
        }

    [   Fact]
        public void Should_return_null_for_ErrorsToString_on_success()
        {
            string code = "code";
            var result = new Result(code);
            Assert.Null(result.ErrorsToString());
        }
    }
}
