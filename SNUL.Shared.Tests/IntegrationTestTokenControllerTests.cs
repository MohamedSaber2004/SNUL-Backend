using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json;
using Commerce.Services.API.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SNUL.Shared.Common.Attributes;
using SNUL.Shared.Common.Options;
using SNUL.Shared.Results;
using Xunit;

namespace SNUL.Shared.Tests;

public sealed class IntegrationTestTokenControllerTests
{
    // Fake placeholders only — never real secrets.
    private const string FlatSecret = "test-only-fake-flat-client-secret-32-chars-ab12";
    private const string LegacySecret = "test-only-fake-legacy-secret-min-32-chars-xyz12";
    private const string Issuer = "snul-integration";
    private const string Audience = "welco-integration";

    [Fact]
    public async Task MintedToken_PassesRealServiceAuthFilter()
    {
        var options = BaseOptions();
        var controller = CreateController(options, "Development");

        var objectResult = Assert.IsType<ObjectResult>(controller.GetToken());
        Assert.Equal(StatusCodes.Status200OK, objectResult.StatusCode);
        var envelope = Assert.IsType<Result<IntegrationTestTokenResponse>>(objectResult.Value);
        Assert.True(envelope.IsSuccess);
        Assert.NotNull(envelope.Data);
        Assert.Equal(3300, envelope.Data!.ExpiresIn);
        Assert.Equal("Bearer", envelope.Data.TokenType);
        Assert.False(string.IsNullOrWhiteSpace(envelope.Data.AccessToken));

        AssertJwtShape(envelope.Data.AccessToken, "snul", FlatSecret, Issuer, Audience, "Egypt");

        var (context, nextCalled) = await ExecuteFilterAsync(options, "Bearer " + envelope.Data.AccessToken);
        Assert.True(nextCalled());
        Assert.Null(context.Result);
    }

    [Fact]
    public void ProductionEnvironment_Returns404_AndDispensesNoToken()
    {
        var options = BaseOptions();
        var controller = CreateController(options, "Production");

        var result = controller.GetToken();

        Assert.Equal(StatusCodes.Status404NotFound, (result as IStatusCodeActionResult)!.StatusCode);
    }

    [Fact]
    public void MisconfiguredSecret_Returns500_WithoutSecretInBody()
    {
        const string shortSecret = "test-short-secret-12345678";
        var options = BaseOptions();
        options.ClientSecret = shortSecret;
        var controller = CreateController(options, "Development");

        var objectResult = Assert.IsType<ObjectResult>(controller.GetToken());

        Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
        var json = JsonSerializer.Serialize(objectResult.Value);
        Assert.DoesNotContain(shortSecret, json);
        Assert.DoesNotContain(FlatSecret, json);
        Assert.DoesNotContain(LegacySecret, json);
        Assert.Contains("misconfig", json, StringComparison.OrdinalIgnoreCase);
    }

    private static IntegrationTestController CreateController(WelcoIntegrationOptions options, string environmentName)
    {
        var controller = new IntegrationTestController(
            null!,
            Options.Create(options),
            new FakeHostEnvironment(environmentName));
        var services = new ServiceCollection();
        services.AddLogging();
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { RequestServices = services.BuildServiceProvider() },
        };
        return controller;
    }

    private static WelcoIntegrationOptions BaseOptions() => new()
    {
        ClientId = "snul",
        ClientSecret = FlatSecret,
        ServiceSecret = LegacySecret,
        ServiceIssuer = Issuer,
        ServiceAudience = Audience,
    };

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
        var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        var context = new AuthorizationFilterContext(actionContext, new List<IFilterMetadata>());
        await new ServiceAuthAttribute().OnAuthorizationAsync(context);
        return (context, () => context.Result is null);
    }

    private sealed class FakeHostEnvironment : Microsoft.AspNetCore.Hosting.IWebHostEnvironment
    {
        public FakeHostEnvironment(string environmentName) => EnvironmentName = environmentName;
        public string EnvironmentName { get; set; }
        public string ApplicationName { get; set; } = "Test";
        public string ContentRootPath { get; set; } = string.Empty;
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
        public string WebRootPath { get; set; } = string.Empty;
        public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
    }
}
