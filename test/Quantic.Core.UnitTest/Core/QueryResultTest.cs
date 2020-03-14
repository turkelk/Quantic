using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Quantic.Core.Test.Core
{
    public class QueryResultTest
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
                var data = 1;

                var result = new QueryResult<int>(data, code, message);
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
                var data = 1;

                var result = new QueryResult<int>(data, code, message);
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
                var failureList = new List<Failure>(null);
                var result = new QueryResult<int>(failureList);
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
                var result = new QueryResult<int>(failureList);
            }
            catch (ArgumentException ex)
            {
                exceptionThrown = true;
                Assert.Equal(paramName, ex.ParamName);
            }

            Assert.True(exceptionThrown);
        }


        [Fact]
        public void Should_success_with_result()
        {
            int result = 1;
            var queryResult = new QueryResult<int>(result);

            Assert.True(queryResult.IsSuccess);
            Assert.Equal(Messages.Success, queryResult.Code);

            Assert.False(queryResult.HasError);
            Assert.False(queryResult.Retry);
        }

        [Fact]
        public void Should_success_with_null_result()
        {
            int? result = null;
            var queryResult = new QueryResult<int?>(result);

            Assert.True(queryResult.IsSuccess);
            Assert.Equal(Messages.Success, queryResult.Code);
            Assert.Equal(result, queryResult.Result);

            Assert.False(queryResult.HasError);
            Assert.False(queryResult.Retry);
        }

        [Fact]
        public void Should_success_with_code_and_result()
        {
            string code = "code";
            int result = 1;

            var queryResult = new QueryResult<int>(result, code);

            Assert.True(queryResult.IsSuccess);
            Assert.Equal(result, queryResult.Result);
            Assert.Equal(code, queryResult.Code);

            Assert.False(queryResult.HasError);
            Assert.False(queryResult.Retry);
        }

        [Fact]
        public void Should_success_with_result_code_and_null_message()
        {
            string code = "code";
            string message = null;
            int result = 1;

            var queryResult = new QueryResult<int>(result, code,message);

            Assert.True(queryResult.IsSuccess);
            Assert.Equal(result, queryResult.Result);
            Assert.Equal(code, queryResult.Code);
            Assert.Equal(message, queryResult.Message);

            Assert.False(queryResult.HasError);
            Assert.False(queryResult.Retry);
        }

        [Fact]
        public void Should_success_with_result_code_and_empty_message()
        {
            string code = "code";
            string message = "";
            int result = 1;

            var queryResult = new QueryResult<int>(result, code, message);

            Assert.True(queryResult.IsSuccess);
            Assert.Equal(result, queryResult.Result);
            Assert.Equal(code, queryResult.Code);
            Assert.Equal(message, queryResult.Message);

            Assert.False(queryResult.HasError);
            Assert.False(queryResult.Retry);
        }

        [Fact]
        public void Should_success_with_failure_without_retry()
        {
            var failireCode = "failure_code";
            var failure = new Failure(failireCode);

            var result = new QueryResult<int>(failure);

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

            var result = new QueryResult<int>(failures);

            Assert.True(result.HasError);
            Assert.False(result.Retry);

            Assert.True(result.Errors.Count == 2
                && result.Errors.Any(x => x.Code == failireCode1)
                && result.Errors.Any(x => x.Code == failireCode2));

            Assert.False(result.IsSuccess);
        }

        [Fact]
        public void Should_success_with_failure_and_false_retry()
        {
            var failireCode = "failure_code";
            var failure = new Failure(failireCode);
            bool retry = false;

            var result = new QueryResult<int>(failure, retry: retry);

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

            var result = new QueryResult<int>(failure, retry: retry);

            Assert.True(result.HasError);
            Assert.True(result.Retry);

            Assert.False(result.IsSuccess);
        }
    }
}
