namespace Quantic.Web
{
    public interface IHeaderValidator
    {
        HttpHeaderValidationResult Validate(string key,string value);    
    }
}