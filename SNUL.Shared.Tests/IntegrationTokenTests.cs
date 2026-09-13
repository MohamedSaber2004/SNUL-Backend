using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Auth.Services.API.Features.Integration.Token;
using SNUL.Shared.Common.Attributes;
using SNUL.Shared.Common.Options;
using SNUL.Shared.Results;
using Xunit;

namespace SNUL.Shared.Tests;

public sealed class IntegrationTokenTests
{
    // Fake placeholders only — never real secrets.
    private const string FlatSecret = "test-only-fake-flat-client-secret-32-chars-ab12";
    private const string SystemSecret = "test-only-fake-system-secret-min-32-chars-ab12";
    private const string LegacySecret = "test-only-fake-legacy-secret-min-32-chars-xyz12";
    private const string WrongSecret = "test-only-fake-WRONG-secret-min-32-chars-zz99";
    private const string Issuer = "snul-integration";
    private const string Audience = "welco-integration";

    [Fact]
    public async Task ValidFlatCredentials_Returns200_WithJwtPassingServiceAuthFilter()
    {
        var options = BaseOptions();
        var handler = CreateHandler(options);

        var result = await handler.Handle(new CreateIntegrationTokenCommand
        {
            ClientId = "snul",
            ClientSecret = FlatSecret,
        }, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        Assert.NotNull(result.Data);
        Assert.Equal(3300, result.Data!.ExpiresIn);
        Assert.Equal("Bearer", result.Data.TokenType);
        Assert.False(string.IsNullOrWhiteSpace(result.Data.AccessToken));

        AssertJwtShape(result.Data.AccessToken, "snul", FlatSecret, Issuer, Audience, "Egypt");

        // The minted token must satisfy SNUL's own [ServiceAuth] validation path.
        var (context, nextCalled) = await ExecuteFilterAsync(options, "Bearer " + result.Data.AccessToken);
        Assert.True(nextCalled());
        Assert.Null(context.Result);
    }

    [Fact]
    public async Task ValidSystemsEntryCredentials_Returns200_WithPerSystemMarketAndIssuer()
    {
        const string market = "UAE";
        var options = BaseOptions();
        options.Systems["welco"] = new WelcoSystemTarget
        {
            BaseUrl = "https://welco.test",
            ClientId = "welco",
            ClientSecret = SystemSecret,
            ServiceSecret = LegacySecret,
            ServiceIssuer = Issuer,
            ServiceAudience = Audience,
            Market = market,
        };
        var handler = CreateHandler(options);

        var result = await handler.Handle(new CreateIntegrationTokenCommand
        {
            ClientId = "welco",
            ClientSecret = SystemSecret,
        }, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        AssertJwtShape(result.Data!.AccessToken, "welco", SystemSecret, Issuer, Audience, market);

        var (context, nextCalled) = await ExecuteFilterAsync(options, "Bearer " + result.Data.AccessToken);
        Assert.True(nextCalled());
        Assert.Null(context.Result);
    }

    [Fact]
    public async Task UnknownClient_And_WrongSecret_ReturnByteIdentical401Envelopes()
    {
        var options = BaseOptions();
        var handler = CreateHandler(options);

        var unknown = await handler.Handle(new CreateIntegrationTokenCommand
        {
            ClientId = "unknown-client",
            ClientSecret = FlatSecret,
        }, CancellationToken.None);

        var wrongSecret = await handler.Handle(new CreateIntegrationTokenCommand
        {
            ClientId = "snul",
            ClientSecret = WrongSecret,
        }, CancellationToken.None);

        AssertUnauthorizedShape(unknown);
        AssertUnauthorizedShape(wrongSecret);
        Assert.Equal(ToJson(unknown), ToJson(wrongSecret));
    }

    [Fact]
    public async Task ShortSecretConfig_Returns401_IdenticalEnvelope()
    {
        var misconfigured = BaseOptions();
        misconfigured.ClientSecret = "test-short-secret-12345678";
        var handler = CreateHandler(misconfigured);

        var result = await handler.Handle(new CreateIntegrationTokenCommand
        {
            ClientId = "snul",
            ClientSecret = FlatSecret,
        }, CancellationToken.None);

        AssertUnauthorizedShape(result);

        var baseline = await CreateHandler(BaseOptions()).Handle(new CreateIntegrationTokenCommand
        {
            ClientId = "unknown-client",
            ClientSecret = FlatSecret,
        }, CancellationToken.None);

        Assert.Equal(ToJson(baseline), ToJson(result));
    }

    [Fact]
    public async Task ExpiredServiceToken_FailsServiceAuthFilter()
    {
        var options = BaseOptions();
        var handler = CreateHandler(options);

        var result = await handler.Handle(new CreateIntegrationTokenCommand
        {
            ClientId = "snul",
            ClientSecret = FlatSecret,
        }, CancellationToken.None);
        Assert.True(result.IsSuccess);

        // Same claims/shape as the issued token, but expired: the filter must 401.
        var expired = MintServiceToken("snul", FlatSecret, Issuer, Audience, "Egypt", DateTime.UtcNow.AddMinutes(-10));
        var (context, nextCalled) = await ExecuteFilterAsync(options, "Bearer " + expired);

        Assert.False(nextCalled());
        Assert.Equal(StatusCodes.Status401Unauthorized, StatusCodeOf(context));
    }

    [Theory]
    [InlineData("", FlatSecret)]
    [InlineData("snul", "")]
    [InlineData("", "")]
    public void Validator_RejectsEmptyCredentials(string clientId, string clientSecret)
    {
        var validator = new CreateIntegrationTokenCommandValidator();

        var validation = validator.Validate(new CreateIntegrationTokenCommand
        {
            ClientId = clientId,
            ClientSecret = clientSecret,
        });

        Assert.False(validation.IsValid);
    }

    [Fact]
    public void Validator_AcceptsNonEmptyCredentials()
    {
        var validator = new CreateIntegrationTokenCommandValidator();

        var validation = validator.Validate(new CreateIntegrationTokenCommand
        {
            ClientId = "snul",
            ClientSecret = FlatSecret,
        });

        Assert.True(validation.IsValid);
    }

    private static CreateIntegrationTokenCommandHandler CreateHandler(WelcoIntegrationOptions options) =>
        new(Options.Create(options), NullLogger<CreateIntegrationTokenCommandHandler>.Instance);

    private static WelcoIntegrationOptions BaseOptions() => new()
    {
        ClientId = "snul",
        ClientSecret = FlatSecret,
        ServiceSecret = LegacySecret,
        ServiceIssuer = Issuer,
        ServiceAudience = Audience,
    };

    private static void AssertUnauthorizedShape(Result<IntegrationTokenResponse> result)
    {
        Assert.False(result.IsSuccess);
        Assert.Equal(StatusCodes.Status401Unauthorized, result.StatusCode);
        Assert.Equal("Invalid client credentials.", result.Message);
        Assert.NotNull(result.Errors);
        var single = Assert.Single(result.Errors);
        Assert.Equal(result.Message, single);
        Assert.Null(result.Data);
    }

    private static string ToJson(Result<IntegrationTokenResponse> result) =>
        JsonSerializer.Serialize(result);

    private static void AssertJwtShape(
        string token, string clientId, string secret, string issuer, string audience, string market)
    {
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

        Assert.Equal(issuer, jwt.Issuer);
        Assert.Equal(audience, jwt.Audiences.Single());
        Assert.Equal("snul-integration", jwt.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value);
        Assert.False(string.IsNullOrWhiteSpace(jwt.Claims.First(c => c.Type == JwtRegisteredClaimNames.Jti).Value));
        Assert.Equal(clientId, jwt.Claims.First(c => c.Type == "client_id").Value);
        Assert.Equal("snul", jwt.Claims.First(c => c.Type == "service").Value);
        Assert.Equal(market, jwt.Claims.First(c => c.Type == "market").Value);
        Assert.DoesNotContain(jwt.Claims, c => c.Type == "azp");
        Assert.Equal(SecurityAlgorithms.HmacSha256, jwt.SignatureAlgorithm);

        var lifetime = jwt.ValidTo - jwt.ValidFrom;
        Assert.True(lifetime >= TimeSpan.FromMinutes(54) && lifetime <= TimeSpan.FromMinutes(56));

        // Signature validates under the configured secret.
        new JwtSecurityTokenHandler().ValidateToken(token, new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
        }, out _);
    }

    private static string MintServiceToken(
        string clientId, string secret, string issuer, string audience, string market, DateTime expires)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, "snul-integration"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("client_id", clientId),
                new Claim("service", "snul"),
                new Claim("market", market),
            },
            notBefore: expires.AddMinutes(-55),
            expires: expires,
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static async Task<(AuthorizationFilterContext Context, Func<bool> NextCalled)> ExecuteFilterAsync(
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
