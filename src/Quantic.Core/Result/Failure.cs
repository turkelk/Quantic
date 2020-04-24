using System;

namespace Quantic.Core
{
    public class Failure
    {
        public Failure(string code, string message)
        {
            Code =  string.IsNullOrEmpty(code)
                ? throw new ArgumentNullException(nameof(code))
                : code;
            Message = message;
        }


        public Failure(string code)
            : this(code, default)
        {

        }

        public string Code { get; }
        public string Message { get; }

        public override string ToString()
        {
            return $"Code:{Code} Message:{Message}";
        }
    }
}
