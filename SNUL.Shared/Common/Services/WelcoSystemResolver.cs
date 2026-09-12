using Microsoft.Extensions.Options;
using SNUL.Shared.Common.Interfaces;
using SNUL.Shared.Common.Options;

namespace SNUL.Shared.Common.Services
{
    /// <summary>
    /// Resolves per-system Welco targets. Unknown or empty input falls back to
    /// <see cref="WelcoIntegrationOptions.DefaultSystem"/>; a configured system missing
    /// from <see cref="WelcoIntegrationOptions.Systems"/> falls back to the legacy
    /// flat properties so current appsettings keep working.
    /// </summary>
    public sealed class WelcoSystemResolver : IWelcoSystemResolver
    {
        private readonly WelcoIntegrationOptions _options;

        public WelcoSystemResolver(IOptions<WelcoIntegrationOptions> options)
        {
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public string Normalize(string? system)
        {
            if (!string.IsNullOrWhiteSpace(system))
            {
                var key = system.Trim();
                if (_options.Systems.ContainsKey(key))
                    return key;
                // Accept unconfigured keys only when they match the default (legacy single-tenant).
                // ArgumentException maps to 400 via CustomExceptionHandlerMiddleware (not 500).
                if (string.Equals(key, _options.DefaultSystem, StringComparison.OrdinalIgnoreCase))
                    return _options.DefaultSystem;
                throw new ArgumentException($"Unknown integration system '{key}'.", nameof(system));
            }

            return string.IsNullOrWhiteSpace(_options.DefaultSystem) ? "snul" : _options.DefaultSystem.Trim();
        }

        public WelcoSystemTarget Resolve(string? system)
        {
            var key = Normalize(system);

            if (_options.Systems.TryGetValue(key, out var target) && target is not null)
                return target;

            // Legacy fallback: flat config acts as the default system's target.
            return new WelcoSystemTarget
            {
                BaseUrl = _options.BaseUrl,
                ServiceSecret = _options.ServiceSecret,
                ServiceIssuer = _options.ServiceIssuer,
                ServiceAudience = _options.ServiceAudience,
                Market = "Egypt",
                TimeoutSeconds = _options.TimeoutSeconds
            };
        }
    }
}
