namespace Quantic.Core
{
    public class ValidationFailure : Failure
    {
        public ValidationFailure(string code, string message) 
            : base(code, message)
        {
        }

        public ValidationFailure(string code)
            : this(code, default)
        {

        }
    }
}
