using Microsoft.Extensions.Options;
using SNUL.Shared.Common.Options;
using SNUL.Shared.Common.Services;
using Xunit;

namespace SNUL.Shared.Tests;

public sealed class WelcoIntegrationFlatCredentialsTests
{
    // Fake placeholders only — never real secrets.
    private const string FlatClientSecret = "test-only-fake-flat-client-secret-32-chars-ab12";
    private const string LegacySecret = "test-only-fake-legacy-secret-min-32-chars-xyz12";

    [Fact]
    public void ResolverLegacyFallback_CarriesFlatClientIdAndClientSecret()
    {
        var options = new WelcoIntegrationOptions
        {
            BaseUrl = "https://welco.test",
            ServiceSecret = LegacySecret,
            ClientId = "snul-flat",
            ClientSecret = FlatClientSecret,
            ServiceIssuer = "snul-integration",
            ServiceAudience = "welco-integration",
            TimeoutSeconds = 30,
            DefaultSystem = "snul",
        };

        var resolver = new WelcoSystemResolver(Options.Create(options));

        var target = resolver.Resolve(null);

        Assert.Equal("snul-flat", target.ClientId);
        Assert.Equal(FlatClientSecret, target.ClientSecret);
    }

    [Fact]
    public void ResolverPrefersSystemsEntry_OverFlatCredentials()
    {
        var systemTarget = new WelcoSystemTarget
        {
            BaseUrl = "https://welco.system.test",
            ClientId = "snul-system",
            ClientSecret = "test-only-fake-system-secret-min-32-chars-ab12",
            ServiceSecret = LegacySecret,
            ServiceIssuer = "snul-integration",
            ServiceAudience = "welco-integration",
            Market = "Egypt",
            TimeoutSeconds = 30,
        };
        var options = new WelcoIntegrationOptions
        {
            BaseUrl = "https://welco.test",
            ServiceSecret = LegacySecret,
            ClientId = "snul-flat",
            ClientSecret = FlatClientSecret,
            ServiceIssuer = "snul-integration",
            ServiceAudience = "welco-integration",
            TimeoutSeconds = 30,
            DefaultSystem = "snul",
        };
        options.Systems["snul"] = systemTarget;

        var resolver = new WelcoSystemResolver(Options.Create(options));

        var target = resolver.Resolve("snul");

        // The per-system entry wins; flat credentials are ignored.
        Assert.Same(systemTarget, target);
        Assert.Equal("snul-system", target.ClientId);
    }
}
