namespace SNUL.Shared.Common.Options
{
    /// <summary>
    /// Resolved credentials and endpoint for a single Welco system (system == market):
    /// the system key selects this target and its <see cref="Market"/> is the market identity.
    /// When no <see cref="WelcoIntegrationOptions.Systems"/> entry matches, the resolver builds
    /// one from the flat <see cref="WelcoIntegrationOptions"/> properties with
    /// <see cref="Market"/> defaulting to "Egypt", so current single-tenant appsettings keep working.
    /// </summary>
    public sealed class WelcoSystemTarget
    {
        public string BaseUrl { get; set; } = string.Empty;
        public string ClientId { get; set; } = "snul";
        public string ClientSecret { get; set; } = string.Empty;
        public string ServiceSecret { get; set; } = string.Empty;
        public string ServiceIssuer { get; set; } = "snul-integration";
        public string ServiceAudience { get; set; } = "welco-integration";
        public string Market { get; set; } = "Egypt";
        public int TimeoutSeconds { get; set; } = 30;
    }
}
