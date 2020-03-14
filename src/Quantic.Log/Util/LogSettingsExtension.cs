namespace Quantic.Log.Util
{
    public static class LogSettingsExtension
    {
        public static bool ShouldLog(this LogSettings logSettings, string requestName)
        {
            return !logSettings.Exclude?.Contains(requestName) ?? true;
        }        
    }
}