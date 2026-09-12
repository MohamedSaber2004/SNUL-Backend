namespace SNUL.Shared.Common.Options
{
        public class WelcoIntegrationOptions
    {
        public const string SectionName = "WelcoIntegration";

        public string BaseUrl { get; set; } = string.Empty;
        public string ServiceSecret { get; set; } = string.Empty;
        public string ServiceIssuer { get; set; } = "snul-integration";
        public string ServiceAudience { get; set; } = "welco-integration";
        public int TokenExpiryMinutes { get; set; } = 55;
        public int TimeoutSeconds { get; set; } = 30;
        public int RetryCount { get; set; } = 3;

        /// <summary>
        /// System key for the default dashboard tenant (used when ?system=/X-System is absent).
        /// </summary>
        public string DefaultSystem { get; set; } = "snul";

        /// <summary>
        /// Per-system endpoints/secrets, keyed by system (e.g. "snul", "welo").
        /// When empty or missing a key, the legacy flat properties above act as fallback
        /// with Market "Egypt", so existing appsettings keep working unchanged.
        /// </summary>
        public Dictionary<string, WelcoSystemTarget> Systems { get; set; } = new(StringComparer.OrdinalIgnoreCase);

                public IntegrationRoutesOptions Routes { get; set; } = new();
    }
}
