namespace Quantic.Log
{
    public class LogSettings
    {
        public LogSettings()
        {
            Settings = new LogSetting[] { };
            GlobalRedactProperties = new string[] { };
            RedactionMask = "***";
        }

        public LogSetting[] Settings { get; set; }

        /// <summary>
        /// Property names to redact from ALL logged requests/responses (case-insensitive).
        /// Example: ["ApiKey", "Password", "AccessToken", "Base64", "Bytes"]
        /// </summary>
        public string[] GlobalRedactProperties { get; set; }

        /// <summary>
        /// Value used when a property is redacted.
        /// </summary>
        public string RedactionMask { get; set; }
    }

    public class LogSetting
    {
        public LogSetting()
        {
            ShouldLog = true;
            LogRequest = true;
            LogResponse = true;
            RedactRequestProperties = new string[] { };
            RedactResponseProperties = new string[] { };
        }

        public bool ShouldLog { get; set; }
        public string Name { get; set; }
        public bool LogRequest { get; set; }
        public bool LogResponse { get; set; }

        /// <summary>
        /// Property names to redact from the logged request object (case-insensitive).
        /// </summary>
        public string[] RedactRequestProperties { get; set; }

        /// <summary>
        /// Property names to redact from the logged response object (case-insensitive).
        /// </summary>
        public string[] RedactResponseProperties { get; set; }
    }
}
