using System;
using Quantic.Core;

namespace Quantic.Log
{
    public class RequestLog
    {
        public string Name { get; set; }
        public object Request { get; set; }
        public object Response { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime ResponseDate { get; set; }
        public string UserCode { get; set; }
        public string CorrelationId { get; set; }
        public Result Result { get; set; }
        public bool Retry { get; set; }
    }

    public enum Result
    {
        Success = 1,
        Error = 2
    }
}
