namespace Quantic.Log
{
    public class LogSettings
    {
        public LogSettings()
        {
            Settings = new LogSetting[] { };
        }

        public LogSetting[] Settings { get; set; }
    }
    public class LogSetting
    {
        public LogSetting()
        {
            ShouldLog = true;
            LogRequest = true;
            LogResponse = true;
        }

        public bool ShouldLog { get; set; }
        public string Name { get; set; }
        public bool LogRequest { get; set; }
        public bool LogResponse { get; set; }
    }
}