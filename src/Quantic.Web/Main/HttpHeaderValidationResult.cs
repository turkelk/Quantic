namespace Quantic.Web
{
    public class HttpHeaderValidationResult
    {
        public static HttpHeaderValidationResult Success = new HttpHeaderValidationResult("success", "Header validation success", true);
        public HttpHeaderValidationResult(string code, string message, bool isSuccess)
        {
            IsSuccess = isSuccess;
            Code = code;
            Message = message;
        }
        public bool IsSuccess { get; }
        public string Code { get; }
        public string Message { get; }
    }
}