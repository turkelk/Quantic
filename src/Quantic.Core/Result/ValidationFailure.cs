namespace Quantic.Core
{
    public class ValidationFailure : Failure
    {
        public ValidationFailureState State { get; }
        public ValidationFailure(string code, string message, ValidationFailureState state)
            : base(code, message)
        {
            this.State = state;
        }
        public ValidationFailure(string code, string message)
            : this(code, message, ValidationFailureState.None)
        {
        }
        public ValidationFailure(string code)
            : this(code, default, ValidationFailureState.None)
        {
        }
    }

    public enum ValidationFailureState
    {
        None,
        Dublication,
        NotFound
    }
}
