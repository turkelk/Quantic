using System;
using System.Collections.Generic;
using Xunit;

namespace Quantic.Core.Test.Core
{
    public class RequestContextTest
    {
        [Fact]
        public void Should_throw_argument_null_exception_for_null_traceId()
        {
            bool exceptionThrown = false;
            string paramName = "traceId";

            try
            {
                string traceId = null;

                Dictionary<string, string> headers = new Dictionary<string, string>()
                {
                    { "header_key","1"}
                };

                var result = new RequestContext(traceId, headers);
            }
            catch (ArgumentNullException ex)
            {
                exceptionThrown = true;
                Assert.Equal(paramName, ex.ParamName);
            }

            Assert.True(exceptionThrown);
        }

        [Fact]
        public void Should_throw_argument_null_exception_for_empty_traceId()
        {
            bool exceptionThrown = false;
            string paramName = "traceId";

            try
            {
                string traceId = "";

                var headers = new Dictionary<string, string>()
                {
                    { "header_key","1"}
                };

                var result = new RequestContext(traceId, headers);
            }
            catch (ArgumentNullException ex)
            {
                exceptionThrown = true;
                Assert.Equal(paramName, ex.ParamName);
            }

            Assert.True(exceptionThrown);
        }

        [Fact]
        public void Should_throw_argument_null_exception_for_null_headers()
        {
            bool exceptionThrown = false;
            string paramName = "headers";

            try
            {
                string traceId = "dummy_trace_id";

                Dictionary<string, string> headers = null;

                var result = new RequestContext(traceId, headers);
            }
            catch (ArgumentNullException ex)
            {
                exceptionThrown = true;
                Assert.Equal(paramName, ex.ParamName);
            }

            Assert.True(exceptionThrown);
        }

        [Fact]
        public void Should_return_accep_language()
        {
            string traceId = "genesis-trace-id";
            string expectedAcceptLanguage = "en";

            Dictionary<string, string> headers = new Dictionary<string, string>
            {
                { "Accept-Language", expectedAcceptLanguage } 
            };

            var result = new RequestContext(traceId, headers);

            Assert.Equal(expectedAcceptLanguage, result.AcceptLanguage);
        }

        [Fact]
        public void Should_return_null_if_accept_language_ismissing_in_header()
        {
            string traceId = "genesis-trace-id";
            string expectedAcceptLanguage = null;

            Dictionary<string, string> headers = new Dictionary<string, string>
            {
                
            };

            var result = new RequestContext(traceId, headers);

            Assert.Equal(expectedAcceptLanguage, result.AcceptLanguage);
        }        

        // [Fact]
        // public void Should_throw_argument_exception_for_missing_unique_referance_code_in_headers()
        // {
        //     bool exceptionThrown = false;
        //     string paramName = "headers";

        //     try
        //     {
        //         string traceId = "dummy_trace_id";

        //         Dictionary<string, string> headers = new Dictionary<string, string>()
        //         {
        //             { "header_key","1"}
        //         };

        //         var result = new RequestContext(traceId, headers);
        //     }
        //     catch (ArgumentException ex)
        //     {
        //         exceptionThrown = true;
        //         Assert.Equal(paramName, ex.ParamName);
        //     }

        //     Assert.True(exceptionThrown);
        // }

        [Fact]
        public void Should_success()
        {
            string traceId = "trace_id";
            //var uniqueReferanceCode = "dummy_ref_code";

            var headers = new Dictionary<string, string>()
            {
                //{ HeaderKeys.UniqueReferanceCode,uniqueReferanceCode}
            };

            var requestContext = new RequestContext(traceId, headers);

            Assert.Equal(traceId, requestContext.TraceId);
            //Assert.Equal(uniqueReferanceCode, requestContext.UniqueReferanceCode);
            Assert.Equal(headers, requestContext.Headers);
        }
    }
}
