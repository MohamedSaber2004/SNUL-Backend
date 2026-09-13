namespace SNUL.Shared.Common.Options
{
    /// <summary>
    /// Flat (legacy single-tenant) Welco integration settings plus optional per-system targets.
    /// </summary>
    /// <remarks>
    /// Credential precedence for JWT signing (highest wins):
    /// 1. The per-system <see cref="Systems"/> entry for the requested system key.
    /// 2. The flat <see cref="ClientId"/> / <see cref="ClientSecret"/> fallback.
    /// 3. The legacy <see cref="ServiceSecret"/> signing fallback when no client secret is configured.
    /// </remarks>
    public class WelcoIntegrationOptions
    {
        public const string SectionName = "WelcoIntegration";

        public string BaseUrl { get; set; } = string.Empty;
        public string ServiceSecret { get; set; } = string.Empty;
        public string ClientId { get; set; } = "snul";
        /// <summary>
        /// Flat-level client secret for the legacy single-tenant fallback.
        /// Only the KEY is committed (empty); the VALUE must come from the
        /// <c>WelcoIntegration__ClientSecret</c> environment variable
        /// (e.g. <c>WelcoIntegration__ClientSecret=YOUR_MIN_32_CHAR_SECRET</c>)
        /// and must never be committed.
        /// Empty in JSON means the legacy <see cref="ServiceSecret"/> signs the token instead.
        /// Precedence: per-system <see cref="Systems"/> entry wins, else this flat value,
        /// else <see cref="ServiceSecret"/> as the signing fallback.
        /// </summary>
        public string ClientSecret { get; set; } = string.Empty;
        public string ServiceIssuer { get; set; } = "snul-integration";
        public string ServiceAudience { get; set; } = "welco-integration";
        public int TokenExpiryMinutes { get; set; } = 55;
        public int TimeoutSeconds { get; set; } = 30;
        public int RetryCount { get; set; } = 3;
        /// <summary>
        /// Default system key used when callers pass a null, empty, or whitespace system.
        /// Each system corresponds to a market (system == market semantics): the key selects
        /// the entry in <see cref="Systems"/> and surfaces as the market identity downstream.
        /// </summary>
        public string DefaultSystem { get; set; } = "snul";

        /// <summary>
        /// Per-system targets keyed by system name (which equals the market).
        /// A matching entry wins over the flat <see cref="ClientId"/> / <see cref="ClientSecret"/>
        /// fallback; a configured <see cref="DefaultSystem"/> with no entry here falls back to
        /// the flat properties with market "Egypt" so current single-tenant appsettings keep working.
        /// </summary>
        public Dictionary<string, WelcoSystemTarget> Systems { get; set; } = new(StringComparer.OrdinalIgnoreCase);

        public IntegrationRoutesOptions Routes { get; set; } = new();
    }
}
