namespace SNUL.Shared.Common.Options
{
    /// <summary>
    /// Configuration for Snul's integration client calling Welco API.
    /// Loaded from appsettings.json section "WelcoIntegration".
    /// </summary>
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
        /// Options pattern configuration for integration routes.
        /// </summary>
        public IntegrationRoutesOptions Routes { get; set; } = new();
    }
}
