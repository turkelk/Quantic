namespace Quantic.Web
{
    public class StringOrNullHeaderValidator : IHeaderValidator
    {
        static StringOrNullHeaderValidator() { }

        private StringOrNullHeaderValidator() { }

        static public StringOrNullHeaderValidator Instance { get; } = new StringOrNullHeaderValidator();
        public HttpHeaderValidationResult Validate(string key,string value)
        {
            if(string.IsNullOrEmpty(value))
            {
                return new HttpHeaderValidationResult("header_value_is_null_or_empty", $"Header value of {key} is null or empty. Value is {value ?? "null"}", false );                
            }
            
            return HttpHeaderValidationResult.Success;
        }
    }
}