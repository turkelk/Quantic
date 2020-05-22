namespace Quantic.Trace.Elastic.Apm
{
    public static class TraceSettingsExtension
    {
        public static bool ShouldCreateSpan(this TraceSettings traceSettings, string requestName)
        {
            return !traceSettings.Exclude?.Contains(requestName) ?? true;
        }
    }
}