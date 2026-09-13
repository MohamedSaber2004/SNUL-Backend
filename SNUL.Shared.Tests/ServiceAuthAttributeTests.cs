using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SNUL.Shared.Common.Attributes;
using SNUL.Shared.Common.Options;
using SNUL.Shared.Results;
using Xunit;

namespace SNUL.Shared.Tests;

public sealed class ServiceAuthAttributeTests
{
    // Fake placeholders only — never real secrets.
    private const string SystemSecret = "test-only-fake-system-secret-min-32-chars-ab12";
    private const string FlatSecret = "test-only-fake-flat-client-secret-32-chars-ab12";
    private const string LegacySecret = "test-only-fake-legacy-secret-min-32-chars-xyz12";
    private const string WrongSecret = "test-only-fake-WRONG-secret-min-32-chars-zz99";
    private const string Issuer = "snul-integration";
    private const string Audience = "welco-integration";

    [Fact]
    public async Task ValidPerClientToken_Passes_InclSystemsEntryResolution()
    {
        // Systems entry wins over the flat secret: token signed with the
        // per-system secret must pass even though the flat secret differs.
        var options = new WelcoIntegrationOptions
        {
            ClientId = "snul",
            ClientSecret = FlatSecret,
            ServiceSecret = LegacySecret,
            ServiceIssuer = Issuer,
            ServiceAudience = Audience,
        };
        options.Systems["welco"] = new WelcoSystemTarget
        {
            BaseUrl = "https://welco.test",
            ClientId = "welco",
            ClientSecret = SystemSecret,
            ServiceSecret = LegacySecret,
            ServiceIssuer = Issuer,
            ServiceAudience = Audience,
        };

        var token = MintServiceToken("welco", SystemSecret, Issuer, Audience, DateTime.UtcNow.AddMinutes(30));
        var (context, nextCalled) = await ExecuteAsync(options, "Bearer " + token);

        Assert.True(nextCalled());
        Assert.Null(context.Result);
    }

    [Fact]
    public async Task UnknownClient_Returns401()
    {
        var options = BaseOptions();
        var token = MintServiceToken("unknown-client", SystemSecret, Issuer, Audience, DateTime.UtcNow.AddMinutes(30));
        var (context, nextCalled) = await ExecuteAsync(options, "Bearer " + token);

        Assert.False(nextCalled());
        Assert.Equal(401, StatusCodeOf(context));
    }

    [Fact]
    public async Task WrongSecret_Returns401()
    {
        var options = BaseOptions();
        var token = MintServiceToken("snul", WrongSecret, Issuer, Audience, DateTime.UtcNow.AddMinutes(30));
        var (context, nextCalled) = await ExecuteAsync(options, "Bearer " + token);

        Assert.False(nextCalled());
        Assert.Equal(401, StatusCodeOf(context));
    }

    [Fact]
    public async Task MissingToken_Returns401()
    {
        var options = BaseOptions();
        var (context, nextCalled) = await ExecuteAsync(options, null);

        Assert.False(nextCalled());
        Assert.Equal(401, StatusCodeOf(context));
    }

    [Fact]
    public async Task MissingClientId_Returns401()
    {
        var options = BaseOptions();
        var token = MintTokenWithoutClientId(SystemSecret, Issuer, Audience, DateTime.UtcNow.AddMinutes(30));
        var (context, nextCalled) = await ExecuteAsync(options, "Bearer " + token);

        Assert.False(nextCalled());
        Assert.Equal(401, StatusCodeOf(context));
    }

    [Fact]
    public async Task Expired_Returns401()
    {
        var options = BaseOptions();
        var token = MintServiceToken("snul", FlatSecret, Issuer, Audience, DateTime.UtcNow.AddMinutes(-10));
        var (context, nextCalled) = await ExecuteAsync(options, "Bearer " + token);

        Assert.False(nextCalled());
        Assert.Equal(401, StatusCodeOf(context));
    }

    [Fact]
    public async Task AdminUserJwt_Returns401()
    {
        var options = BaseOptions();
        var token = MintAdminUserJwt();
        var (context, nextCalled) = await ExecuteAsync(options, "Bearer " + token);

        Assert.False(nextCalled());
        Assert.Equal(401, StatusCodeOf(context));
    }

    [Fact]
    public async Task ValidFlatClientSecretToken_Passes()
    {
        // Flat (legacy single-tenant) path: no Systems entry, token signed
        // with the flat ClientSecret must pass.
        var options = BaseOptions();
        var token = MintServiceToken("snul", FlatSecret, Issuer, Audience, DateTime.UtcNow.AddMinutes(30));
        var (context, nextCalled) = await ExecuteAsync(options, "Bearer " + token);

        Assert.True(nextCalled());
        Assert.Null(context.Result);
    }

    [Fact]
    public async Task ServiceSecretFallback_Passes_WhenClientSecretEmpty()
    {
        // Resolver-equivalent flat setup: empty ClientSecret falls back to
        // ServiceSecret for signing/validation.
        var options = BaseOptions();
        options.ClientSecret = string.Empty;
        var token = MintServiceToken("snul", LegacySecret, Issuer, Audience, DateTime.UtcNow.AddMinutes(30));
        var (context, nextCalled) = await ExecuteAsync(options, "Bearer " + token);

        Assert.True(nextCalled());
        Assert.Null(context.Result);
    }

    [Fact]
    public async Task AdminUserJwt_WithSpoofedClientIdAndWrongSecret_Returns401()
    {
        var options = BaseOptions();
        var token = MintAdminUserJwtWithSpoofedClientId("snul", WrongSecret, Issuer, Audience);
        var (context, nextCalled) = await ExecuteAsync(options, "Bearer " + token);

        Assert.False(nextCalled());
        Assert.Equal(401, StatusCodeOf(context));
        AssertUnauthorizedEnvelope(context);
    }

    [Fact]
    public async Task WrongIssuer_Returns401()
    {
        var options = BaseOptions();
        var token = MintServiceToken("snul", FlatSecret, "wrong-issuer", Audience, DateTime.UtcNow.AddMinutes(30));
        var (context, nextCalled) = await ExecuteAsync(options, "Bearer " + token);

        Assert.False(nextCalled());
        Assert.Equal(401, StatusCodeOf(context));
        AssertUnauthorizedEnvelope(context);
    }

    [Fact]
    public async Task ShortSecretConfig_Returns401()
    {
        // Fail closed: a configured secret under 32 chars is misconfiguration
        // and must 401. Token is minted with the otherwise-valid flat secret
        // so the 401 proves the short configured value fails closed
        // (HS256 minting itself requires >=256 bits, so we cannot mint with
        // the short value directly).
        const string shortSecret = "test-short-secret-12345678";
        var options = BaseOptions();
        options.ClientSecret = shortSecret;
        var token = MintServiceToken("snul", FlatSecret, Issuer, Audience, DateTime.UtcNow.AddMinutes(30));
        var (context, nextCalled) = await ExecuteAsync(options, "Bearer " + token);

        Assert.False(nextCalled());
        Assert.Equal(401, StatusCodeOf(context));
        AssertUnauthorizedEnvelope(context);
    }

    [Fact]
    public async Task UnauthorizedBody_MatchesResultEnvelope()
    {
        var options = BaseOptions();
        var (context, nextCalled) = await ExecuteAsync(options, null);

        Assert.False(nextCalled());
        Assert.Equal(401, StatusCodeOf(context));
        AssertUnauthorizedEnvelope(context);
    }

    private static WelcoIntegrationOptions BaseOptions() => new()
    {
        ClientId = "snul",
        ClientSecret = FlatSecret,
        ServiceSecret = LegacySecret,
        ServiceIssuer = Issuer,
        ServiceAudience = Audience,
    };

    private static string MintServiceToken(string clientId, string secret, string issuer, string audience, DateTime expires)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: new[] { new Claim("client_id", clientId) },
            notBefore: expires.AddMinutes(-35),
            expires: expires,
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string MintTokenWithoutClientId(string secret, string issuer, string audience, DateTime expires)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: new[] { new Claim(JwtRegisteredClaimNames.Sub, "snul-integration") },
            notBefore: expires.AddMinutes(-35),
            expires: expires,
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string MintAdminUserJwt()
    {
        // Typical user JWT: different issuer/audience/secret, role claim, no client_id.
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("test-only-fake-user-jwt-secret-min-32-chars-aaaa"));
        var token = new JwtSecurityToken(
            issuer: "snul-auth",
            audience: "snul-api",
            claims: new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, "admin-user-id"),
                new Claim(ClaimTypes.Role, "Admin"),
            },
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string MintAdminUserJwtWithSpoofedClientId(string clientId, string secret, string issuer, string audience)
    {
        // Admin user JWT attempting to spoof a service client_id: must still fail
        // closed because the signature does not match the configured service secret.
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, "admin-user-id"),
                new Claim(ClaimTypes.Role, "Admin"),
                new Claim("client_id", clientId),
            },
            notBefore: DateTime.UtcNow.AddMinutes(-5),
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static void AssertUnauthorizedEnvelope(AuthorizationFilterContext context)
    {
        var json = Assert.IsType<JsonResult>(context.Result);
        Assert.Equal(StatusCodes.Status401Unauthorized, json.StatusCode);
        var body = Assert.IsType<Result<object?>>(json.Value);
        Assert.False(body.IsSuccess);
        Assert.Equal(StatusCodes.Status401Unauthorized, body.StatusCode);
        Assert.False(string.IsNullOrWhiteSpace(body.Message));
        Assert.NotNull(body.Errors);
        var single = Assert.Single(body.Errors);
        Assert.Equal(body.Message, single);
        Assert.Null(body.Data);
    }

    private static async Task<(AuthorizationFilterContext Context, Func<bool> NextCalled)> ExecuteAsync(
        WelcoIntegrationOptions options, string? authorizationHeader)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton(Options.Create(options));
        var provider = services.BuildServiceProvider();

        var httpContext = new DefaultHttpContext { RequestServices = provider };
        if (authorizationHeader is not null)
            httpContext.Request.Headers["Authorization"] = authorizationHeader;

        var actionContext = new ActionContext(
            httpContext, new RouteData(), new ActionDescriptor());
        var context = new AuthorizationFilterContext(
            actionContext, new List<IFilterMetadata>());

        await new ServiceAuthAttribute().OnAuthorizationAsync(context);
        return (context, () => context.Result is null);
    }

    private static int? StatusCodeOf(AuthorizationFilterContext context) =>
        (context.Result as IStatusCodeActionResult)?.StatusCode;
}
