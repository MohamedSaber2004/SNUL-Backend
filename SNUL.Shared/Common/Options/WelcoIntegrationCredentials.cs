namespace SNUL.Shared.Common.Options
{
    /// <summary>
    /// Single canonical credential-resolution path for Welco service auth.
    /// Both the <c>ServiceAuth</c> filter (validate by client_id) and the
    /// token-fetch path (sign/post by resolved target) resolve the secret as
    /// <c>ClientSecret if set, else ServiceSecret</c>, and resolve the expected
    /// issuer/audience as <c>target override if set, else flat option</c>.
    /// </summary>
    /// <remarks>
    /// Choice: key-only resolution is canonical. The filter's former
    /// scan-<c>Systems.Values</c>-by-<c>ClientId</c> branch was removed so both
    /// paths agree: generation resolves by system key via
    /// <c>WelcoSystemResolver</c>, and validation resolves by token client_id
    /// via dictionary key lookup plus the flat <c>ClientId</c> fallback.
    /// Invariant: a <c>Systems</c> dictionary key equals its
    /// <c>ClientId</c> (e.g. key "welco" holds <c>ClientId == "welco"</c>).
    /// Configurations that violate the invariant fail closed as unknown
    /// clients; currently-valid tokens (key == ClientId, or flat fallback)
    /// are unaffected.
    /// Expected issuer/audience semantics are unchanged (today:
    /// "snul-integration" / "welco-integration" defaults with per-system
    /// overrides); callers must fail closed on empty values, never disable
    /// validation.
    /// </remarks>
    public static class WelcoIntegrationCredentials
    {
        /// <summary>
        /// Shared secret selection: per-target <c>ClientSecret</c> wins,
        /// else the legacy <c>ServiceSecret</c> fallback.
        /// </summary>
        public static string ResolveSecret(WelcoSystemTarget target)
        {
            ArgumentNullException.ThrowIfNull(target);
            return string.IsNullOrWhiteSpace(target.ClientSecret)
                ? (target.ServiceSecret ?? string.Empty)
                : target.ClientSecret;
        }

        /// <summary>
        /// Resolves secret/issuer/audience for a token client_id using the
        /// canonical key-only lookup plus flat fallback. Returns false for
        /// unknown clients; callers fail closed on empty/short values.
        /// </summary>
        public static bool TryResolveByClientId(
            WelcoIntegrationOptions options,
            string? clientId,
            out string secret,
            out string expectedIssuer,
            out string expectedAudience)
        {
            secret = string.Empty;
            expectedIssuer = string.Empty;
            expectedAudience = string.Empty;

            if (options is null || string.IsNullOrWhiteSpace(clientId))
                return false;

            var id = clientId.Trim();

            WelcoSystemTarget? target = null;
            if (options.Systems.TryGetValue(id, out var byKey) && byKey is not null)
            {
                target = byKey;
            }
            else if (string.Equals(options.ClientId, id, StringComparison.OrdinalIgnoreCase))
            {
                target = null;
            }
            else
            {
                return false;
            }

            if (target is not null)
            {
                secret = ResolveSecret(target);
                expectedIssuer = string.IsNullOrWhiteSpace(target.ServiceIssuer)
                    ? (options.ServiceIssuer ?? string.Empty)
                    : target.ServiceIssuer;
                expectedAudience = string.IsNullOrWhiteSpace(target.ServiceAudience)
                    ? (options.ServiceAudience ?? string.Empty)
                    : target.ServiceAudience;
            }
            else
            {
                secret = string.IsNullOrWhiteSpace(options.ClientSecret)
                    ? (options.ServiceSecret ?? string.Empty)
                    : options.ClientSecret;
                expectedIssuer = options.ServiceIssuer ?? string.Empty;
                expectedAudience = options.ServiceAudience ?? string.Empty;
            }

            return true;
        }
    }
}
