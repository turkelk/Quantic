namespace Quantic.FeatureManagement
{
    public interface IHandlerFeatureInfoProvider
    {
        HandlerFeatureInfo GetHandlerInfo(string name);
    }
}