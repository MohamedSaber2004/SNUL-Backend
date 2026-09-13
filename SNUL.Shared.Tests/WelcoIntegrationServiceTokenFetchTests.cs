using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using SNUL.Shared.Common.Options;
using SNUL.Shared.Common.Services;
using SNUL.Shared.Common.DTOs.Integration;
using SNUL.Shared.Results;
using Xunit;

namespace SNUL.Shared.Tests;

public sealed class WelcoIntegrationServiceTokenFetchTests
{
    // Fake placeholders only — never real secrets.
    private const string ClientSecret = "test-only-fake-client-secret-min-32-chars-abcdef";
    private const string LegacySecret = "test-only-fake-legacy-secret-min-32-chars-xyz12";

    [Fact]
    public async Task ConsecutiveCalls_ReuseCachedToken_SinglePost_AndBearerCarriesFetchedToken()
    {
        var target = TestTarget();
        var handler = new TokenRouteHandler(
            tokenResponder: _ => TokenEnvelope("fetched-token-abc", 3300));
        var service = CreateService(target, handler);

        await service.GetProvidersAsync(system: "snul");
        await service.GetProvidersAsync(system: "snul");

        Assert.Equal(1, handler.TokenPostCount);
        Assert.Equal("fetched-token-abc", handler.LastBearerToken);

        // Credentials posted as camelCase clientId/clientSecret.
        var body = handler.LastTokenRequestBody ?? string.Empty;
        Assert.Contains("snul", body);
        Assert.Contains("clientId", body);
        Assert.Contains("clientSecret", body);
    }

    [Fact]
    public async Task TokenEndpoint401_ThrowsUnauthorized_AndClearsCache()
    {
        var target = TestTarget();
        var handler = new TokenRouteHandler(
            tokenResponder: _ => new HttpResponseMessage(HttpStatusCode.Unauthorized)
            {
                Content = new StringContent(
                    """{"isSuccess":false,"statusCode":401,"message":"Invalid client credentials.","errors":[],"data":null}""",
                    Encoding.UTF8, "application/json"),
            });
        var service = CreateService(target, handler);

        var ex1 = await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => service.GetServiceTokenAsync(target, CancellationToken.None));
        Assert.Contains("client credentials", ex1.Message, StringComparison.OrdinalIgnoreCase);

        var ex2 = await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => service.GetServiceTokenAsync(target, CancellationToken.None));

        // Cache was cleared: the second call re-POSTed instead of replaying a cached failure.
        Assert.Equal(2, handler.TokenPostCount);
    }

    [Fact]
    public async Task GetProvidersAsync_Token401_PropagatesUnauthorized_WithoutFallbackJwt()
    {
        var target = TestTarget();
        var handler = new TokenRouteHandler(
            tokenResponder: _ => new HttpResponseMessage(HttpStatusCode.Unauthorized)
            {
                Content = new StringContent(
                    """{"isSuccess":false,"statusCode":401,"message":"Invalid client credentials.","errors":[],"data":null}""",
                    Encoding.UTF8, "application/json"),
            });
        var service = CreateService(target, handler);

        var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => service.GetProvidersAsync(system: "snul"));

        Assert.Contains("client credentials", ex.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Equal(1, handler.TokenPostCount);
        // No fallback JWT was minted and no data-endpoint call was attempted.
        Assert.Equal(0, handler.DataCallCount);
        Assert.Null(handler.LastBearerToken);
    }

    [Fact]
    public async Task GetProvidersAsync_CallerCancelled_PropagatesOperationCanceled_WithoutFallback()
    {
        var target = TestTarget();
        var handler = new TokenRouteHandler(
            tokenResponder: _ => TokenEnvelope("fetched-token-abc", 3300));
        var service = CreateService(target, handler);
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => service.GetProvidersAsync(cts.Token));

        // Cancelled before any network work: no token POST, no data call, no fallback JWT.
        Assert.Equal(0, handler.TokenPostCount);
        Assert.Equal(0, handler.DataCallCount);
        Assert.Null(handler.LastBearerToken);
    }

    [Fact]
    public async Task TokenEndpointDown_PropagatesAsUnavailable_WithoutFallbackJwt()
    {
        var target = TestTarget();
        var handler = new TokenRouteHandler(
            tokenResponder: _ => throw new HttpRequestException("connection refused"));
        var service = CreateService(target, handler);

        var result = await service.GetProvidersAsync(system: "snul");

        // No local-mint fallback: the fetch failure propagates through Send*
        // try/catch and maps to 502 via HandleException.
        Assert.False(result.IsSuccess);
        Assert.Equal(502, result.StatusCode);
        Assert.Equal(0, handler.DataCallCount);
        Assert.Null(handler.LastBearerToken);
    }

    [Fact]
    public async Task TokenCache_IsolatesPerSystem()
    {
        var targetA = TestTarget("https://welco-a.test");
        var targetB = TestTarget("https://welco-b.test");
        var handler = new TokenRouteHandler(
            tokenResponder: req => TokenEnvelope(
                req.RequestUri!.Host.Contains('a') ? "token-a" : "token-b", 3300));
        var options = new WelcoIntegrationOptions
        {
            ServiceSecret = LegacySecret,
            DefaultSystem = "snul",
            TokenExpiryMinutes = 55,
            TimeoutSeconds = 30,
            RetryCount = 0,
        };
        options.Systems["a"] = targetA;
        options.Systems["b"] = targetB;
        var optionsWrapper = Options.Create(options);
        var resolver = new WelcoSystemResolver(optionsWrapper);
        var http = new HttpClient(handler) { BaseAddress = new Uri("https://welco-a.test/") };
        var service = new WelcoIntegrationService(http, optionsWrapper, resolver);

        await service.GetProvidersAsync(system: "a");
        await service.GetProvidersAsync(system: "b");
        await service.GetProvidersAsync(system: "a");

        Assert.Equal(2, handler.TokenPostCount);
    }

    private static WelcoSystemTarget TestTarget(string baseUrl = "https://welco.test") =>
        new()
        {
            BaseUrl = baseUrl,
            ClientId = "snul",
            ClientSecret = ClientSecret,
            ServiceSecret = LegacySecret,
            ServiceIssuer = "snul-integration",
            ServiceAudience = "welco-integration",
            Market = "Egypt",
            TimeoutSeconds = 30,
        };

    private static WelcoIntegrationService CreateService(WelcoSystemTarget target, TokenRouteHandler handler)
    {
        var options = new WelcoIntegrationOptions
        {
            ServiceSecret = LegacySecret,
            DefaultSystem = "snul",
            TokenExpiryMinutes = 55,
            TimeoutSeconds = 30,
            RetryCount = 0,
        };
        options.Systems["snul"] = target;
        var optionsWrapper = Options.Create(options);
        var resolver = new WelcoSystemResolver(optionsWrapper);
        var http = new HttpClient(handler) { BaseAddress = new Uri("https://welco.test/") };
        return new WelcoIntegrationService(http, optionsWrapper, resolver);
    }

    private static HttpResponseMessage TokenEnvelope(string accessToken, int expiresIn) =>
        new(HttpStatusCode.OK)
        {
            Content = new StringContent(
                JsonSerializer.Serialize(new
                {
                    isSuccess = true,
                    statusCode = 200,
                    message = "ok",
                    errors = Array.Empty<string>(),
                    data = new { accessToken, expiresIn, tokenType = "Bearer" },
                }),
                Encoding.UTF8, "application/json"),
        };

    private sealed class TokenRouteHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _tokenResponder;

        public TokenRouteHandler(Func<HttpRequestMessage, HttpResponseMessage> tokenResponder)
        {
            _tokenResponder = tokenResponder;
        }

        public int TokenPostCount { get; private set; }
        public int DataCallCount { get; private set; }
        public string? LastBearerToken { get; private set; }
        public string? LastTokenRequestBody { get; private set; }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var uri = request.RequestUri?.ToString() ?? string.Empty;
            if (uri.Contains("api/integration/token", StringComparison.OrdinalIgnoreCase))
            {
                TokenPostCount++;
                if (request.Content is not null)
                    LastTokenRequestBody = await request.Content.ReadAsStringAsync(cancellationToken);
                return _tokenResponder(request);
            }

            LastBearerToken = request.Headers.Authorization?.Parameter;
            DataCallCount++;
            var payload = JsonSerializer.Serialize(Result<List<ExternalProviderDto>>.Success(new()));
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(payload, Encoding.UTF8, "application/json"),
            };
        }
    }
}
