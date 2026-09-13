using System.Collections.Concurrent;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using SNUL.Shared.Common.DTOs.Integration;
using SNUL.Shared.Common.Interfaces;
using SNUL.Shared.Common.Options;
using SNUL.Shared.Localization;
using SNUL.Shared.Results;

namespace SNUL.Shared.Common.Services
{
    public class WelcoIntegrationService : IWelcoIntegrationService
    {
        private static readonly JsonSerializerOptions TokenJsonOptions = new(JsonSerializerDefaults.Web)
        {
            PropertyNameCaseInsensitive = true,
        };

        private readonly HttpClient _http;
        private readonly WelcoIntegrationOptions _options;
        private readonly IWelcoSystemResolver _resolver;
        private readonly ILogger<WelcoIntegrationService> _logger;
        private readonly ConcurrentDictionary<string, CachedToken> _tokenCache = new(StringComparer.OrdinalIgnoreCase);
        // Bounded: keys are BaseUrl|clientId from configured systems, not user input.
        private readonly ConcurrentDictionary<string, SemaphoreSlim> _tokenLocks = new(StringComparer.OrdinalIgnoreCase);
        public WelcoIntegrationService(
            HttpClient http,
            IOptions<WelcoIntegrationOptions> options,
            IWelcoSystemResolver resolver,
            ILogger<WelcoIntegrationService>? logger = null)
        {
            _http = http;
            _options = options.Value;
            _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
            _logger = logger ?? NullLogger<WelcoIntegrationService>.Instance;
        }

        public async Task<string> GetServiceTokenAsync(WelcoSystemTarget target, CancellationToken ct = default)
        {
            ArgumentNullException.ThrowIfNull(target);
            var key = GetCacheKey(target);
            if (_tokenCache.TryGetValue(key, out var cached) && cached.ExpiresAt > DateTimeOffset.UtcNow)
                return cached.AccessToken;

            var gate = _tokenLocks.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));
            await gate.WaitAsync(ct).ConfigureAwait(false);
            try
            {
                if (_tokenCache.TryGetValue(key, out cached) && cached.ExpiresAt > DateTimeOffset.UtcNow)
                    return cached.AccessToken;

                var (accessToken, expiresIn) = await FetchServiceTokenAsync(target, key, ct).ConfigureAwait(false);
                var ttl = TimeSpan.FromSeconds(expiresIn);
                var effective = ttl - TimeSpan.FromMinutes(5);
                if (effective <= TimeSpan.Zero)
                    effective = ttl;
                _tokenCache[key] = new CachedToken(accessToken, DateTimeOffset.UtcNow + effective);
                return accessToken;
            }
            finally
            {
                gate.Release();
            }
        }

        private async Task<(string AccessToken, int ExpiresIn)> FetchServiceTokenAsync(
            WelcoSystemTarget target, string cacheKey, CancellationToken ct)
        {
            var clientId = string.IsNullOrWhiteSpace(target.ClientId) ? "snul" : target.ClientId;
            var secret = WelcoIntegrationCredentials.ResolveSecret(target);
            var attempts = Math.Max(1, _options.RetryCount + 1);
            var timeoutSeconds = target.TimeoutSeconds > 0
                ? target.TimeoutSeconds
                : _options.TimeoutSeconds > 0 ? _options.TimeoutSeconds : 30;

            for (var attempt = 1; attempt <= attempts; attempt++)
            {
                ct.ThrowIfCancellationRequested();
                try
                {
                    using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
                    linkedCts.CancelAfter(TimeSpan.FromSeconds(Math.Max(1, timeoutSeconds)));

                    using var req = new HttpRequestMessage(HttpMethod.Post, BuildUri(target, _options.Routes.TokenRoute));
                    req.Content = JsonContent.Create(new { clientId, clientSecret = secret });

                    using var resp = await _http.SendAsync(req, linkedCts.Token).ConfigureAwait(false);
                    if (resp.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        _tokenCache.TryRemove(cacheKey, out _);
                        _logger.LogWarning("Welco token endpoint rejected the configured client credentials for ClientId={ClientId} (401).", clientId);
                        throw new UnauthorizedAccessException(
                            "Welco token endpoint rejected the configured client credentials (401 Invalid client credentials). Check the symmetric WelcoIntegration ClientId/ClientSecret for this system.");
                    }

                    if (!resp.IsSuccessStatusCode)
                    {
                        _logger.LogWarning("Welco token endpoint returned {StatusCode} for ClientId={ClientId} (attempt {Attempt}/{Attempts}).", (int)resp.StatusCode, clientId, attempt, attempts);
                        throw new HttpRequestException($"Welco token endpoint returned {(int)resp.StatusCode}.");
                    }

                    var envelope = await resp.Content
                        .ReadFromJsonAsync<TokenResponseEnvelope>(TokenJsonOptions, linkedCts.Token)
                        .ConfigureAwait(false);
                    var accessToken = envelope?.Data?.AccessToken;
                    var expiresIn = envelope?.Data?.ExpiresIn ?? 0;
                    if (string.IsNullOrWhiteSpace(accessToken))
                    {
                        _logger.LogWarning("Welco token endpoint returned an empty access token for ClientId={ClientId}.", clientId);
                        throw new InvalidOperationException("Welco token endpoint returned an empty access token.");
                    }
                    if (expiresIn <= 0)
                    {
                        _logger.LogWarning("Welco token endpoint returned an invalid expiry for ClientId={ClientId}.", clientId);
                        throw new InvalidOperationException("Welco token endpoint returned an invalid expiry.");
                    }
                    return (accessToken, expiresIn);
                }
                catch (UnauthorizedAccessException)
                {
                    throw;
                }
                catch (OperationCanceledException) when (ct.IsCancellationRequested)
                {
                    throw;
                }
                catch (OperationCanceledException ex) when (!ct.IsCancellationRequested)
                {
                    // Per-attempt timeout: retry while attempts remain, else surface as unreachable.
                    if (attempt == attempts)
                    {
                        _logger.LogWarning(ex, "Welco token endpoint timed out for ClientId={ClientId} after {Attempts} attempt(s).", clientId, attempts);
                        throw new HttpRequestException("Welco token endpoint timed out.", ex);
                    }

                    _logger.LogWarning("Welco token endpoint attempt {Attempt}/{Attempts} timed out for ClientId={ClientId}; retrying.", attempt, attempts, clientId);
                }
                catch (HttpRequestException ex)
                {
                    if (attempt == attempts)
                    {
                        _logger.LogWarning(ex, "Welco token endpoint request failed for ClientId={ClientId} after {Attempts} attempt(s).", clientId, attempts);
                        throw;
                    }

                    _logger.LogWarning("Welco token endpoint attempt {Attempt}/{Attempts} failed for ClientId={ClientId}; retrying.", attempt, attempts, clientId);
                }
            }

            _logger.LogWarning("Welco token endpoint is unreachable for ClientId={ClientId}.", clientId);
            throw new HttpRequestException("Welco token endpoint is unreachable.");
        }

        private static string GetCacheKey(WelcoSystemTarget target)
        {
            var clientId = string.IsNullOrWhiteSpace(target.ClientId) ? "snul" : target.ClientId.Trim().ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(target.BaseUrl))
                return "legacy|" + clientId;
            return target.BaseUrl.Trim().TrimEnd('/').ToLowerInvariant() + "|" + clientId;
        }

        private sealed record CachedToken(string AccessToken, DateTimeOffset ExpiresAt);

        private sealed class TokenResponseEnvelope
        {
            public TokenResponseData? Data { get; set; }
        }

        private sealed class TokenResponseData
        {
            public string? AccessToken { get; set; }
            public int ExpiresIn { get; set; }
            public string? TokenType { get; set; }
        }

        public Task<Result<List<ExternalProviderDto>>> GetProvidersAsync(CancellationToken ct = default, string? system = null)
        {
            var route = _options.Routes.ProvidersBase;
            return SendGetAsync<List<ExternalProviderDto>>(route, ct, system);
        }

        public Task<Result<List<ExternalProductDto>>> GetProductsAsync(int page = 1, int pageSize = 50, CancellationToken ct = default, string? system = null)
        {
            var route = $"{_options.Routes.ProductsBase}?page={page}&pageSize={pageSize}";
            return SendGetAsync<List<ExternalProductDto>>(route, ct, system);
        }

        public Task<Result<ExternalProductDto>> GetProductByIdAsync(Guid welcoProductId, CancellationToken ct = default, string? system = null)
        {
            var subRoute = string.Format(_options.Routes.ProductsGetById, welcoProductId);
            var route = CombineRoute(_options.Routes.ProductsBase, subRoute);
            return SendGetAsync<ExternalProductDto>(route, ct, system);
        }

        public Task<Result<InventoryCheckResponse>> CheckInventoryAsync(InventoryCheckRequest request, CancellationToken ct = default, string? system = null)
        {
            var route = CombineRoute(_options.Routes.InventoryBase, _options.Routes.InventoryCheck);
            return SendPostAsync<InventoryCheckRequest, InventoryCheckResponse>(route, request, ct, system);
        }

        public Task<Result<InventoryCheckResponse>> ReserveInventoryAsync(InventoryCheckRequest request, CancellationToken ct = default, string? system = null)
        {
            var route = CombineRoute(_options.Routes.InventoryBase, _options.Routes.InventoryReserve);
            return SendPostAsync<InventoryCheckRequest, InventoryCheckResponse>(route, request, ct, system);
        }

        public Task<Result<ExternalOrderResponse>> CreateOrderAsync(CreateExternalOrderRequest request, CancellationToken ct = default, string? system = null)
        {
            var route = _options.Routes.OrdersBase;
            return SendPostAsync<CreateExternalOrderRequest, ExternalOrderResponse>(route, request, ct, system);
        }

        public Task<Result<ExternalOrderResponse>> GetOrderByIdAsync(Guid welcoOrderId, CancellationToken ct = default, string? system = null)
        {
            var subRoute = string.Format(_options.Routes.OrdersGetById, welcoOrderId);
            var route = CombineRoute(_options.Routes.OrdersBase, subRoute);
            return SendGetAsync<ExternalOrderResponse>(route, ct, system);
        }

        public Task<Result<bool>> UpdateOrderStatusAsync(Guid welcoOrderId, UpdateExternalStatusRequest request, CancellationToken ct = default, string? system = null)
        {
            var subRoute = string.Format(_options.Routes.OrdersUpdateStatus, welcoOrderId);
            var route = CombineRoute(_options.Routes.OrdersBase, subRoute);
            return SendPutAsync<UpdateExternalStatusRequest, bool>(route, request, ct, system);
        }

        public Task<Result<ExternalQuoteResponse>> CreateQuoteAsync(CreateExternalQuoteRequest request, CancellationToken ct = default, string? system = null)
        {
            var route = _options.Routes.QuotesBase;
            return SendPostAsync<CreateExternalQuoteRequest, ExternalQuoteResponse>(route, request, ct, system);
        }

        public Task<Result<bool>> UpdateQuoteStatusAsync(Guid welcoQuoteId, UpdateExternalStatusRequest request, CancellationToken ct = default, string? system = null)
        {
            var subRoute = string.Format(_options.Routes.QuotesUpdateStatus, welcoQuoteId);
            var route = CombineRoute(_options.Routes.QuotesBase, subRoute);
            return SendPutAsync<UpdateExternalStatusRequest, bool>(route, request, ct, system);
        }

        public Task<Result<List<ExternalCategoryDto>>> GetCategoriesAsync(CancellationToken ct = default, string? system = null)
        {
            var route = _options.Routes.CategoriesBase;
            return SendGetAsync<List<ExternalCategoryDto>>(route, ct, system);
        }

        public Task<Result<ExternalCategoryDto>> GetCategoryByIdAsync(Guid welcoCategoryId, CancellationToken ct = default, string? system = null)
        {
            var subRoute = string.Format(_options.Routes.CategoriesGetById, welcoCategoryId);
            var route = CombineRoute(_options.Routes.CategoriesBase, subRoute);
            return SendGetAsync<ExternalCategoryDto>(route, ct, system);
        }

        public Task<Result<DistributorApplicationDto>> SubmitDistributorApplicationAsync(ApplyDistributorRequest request, CancellationToken ct = default, string? system = null)
        {
            var route = CombineRoute(_options.Routes.DistributorsBase, _options.Routes.DistributorsApply);
            return SendPostAsync<ApplyDistributorRequest, DistributorApplicationDto>(route, request, ct, system);
        }

        public Task<Result<List<DistributorApplicationDto>>> GetDistributorApplicationsAsync(CancellationToken ct = default, string? system = null)
        {
            var route = _options.Routes.DistributorsBase;
            return SendGetAsync<List<DistributorApplicationDto>>(route, ct, system);
        }

        public Task<Result<DistributorApplicationDto>> GetDistributorApplicationByIdAsync(Guid id, CancellationToken ct = default, string? system = null)
        {
            var subRoute = string.Format(_options.Routes.DistributorsGetById, id);
            var route = CombineRoute(_options.Routes.DistributorsBase, subRoute);
            return SendGetAsync<DistributorApplicationDto>(route, ct, system);
        }

        public Task<Result<bool>> ApproveDistributorApplicationAsync(Guid id, CancellationToken ct = default, string? system = null)
        {
            var subRoute = string.Format(_options.Routes.DistributorsApprove, id);
            var route = CombineRoute(_options.Routes.DistributorsBase, subRoute);
            return SendPutAsync<object?, bool>(route, null, ct, system);
        }

        public Task<Result<bool>> RejectDistributorApplicationAsync(Guid id, string? reason = null, CancellationToken ct = default, string? system = null)
        {
            var subRoute = string.Format(_options.Routes.DistributorsReject, id);
            var route = CombineRoute(_options.Routes.DistributorsBase, subRoute);
            return SendPutAsync<object, bool>(route, new { Reason = reason }, ct, system);
        }

        public Task<Result<List<ExternalSupportTicketDto>>> GetSupportTicketsAsync(string? status = null, CancellationToken ct = default, string? system = null)
        {
            var subRoute = _options.Routes.SupportTickets;
            if (!string.IsNullOrWhiteSpace(status))
                subRoute += $"?status={Uri.EscapeDataString(status)}";
            var route = CombineRoute(_options.Routes.SupportBase, subRoute);
            return SendGetAsync<List<ExternalSupportTicketDto>>(route, ct, system);
        }

        public Task<Result<ExternalSupportTicketDto>> GetSupportTicketByIdAsync(Guid id, CancellationToken ct = default, string? system = null)
        {
            var subRoute = string.Format(_options.Routes.SupportTicketById, id);
            var route = CombineRoute(_options.Routes.SupportBase, subRoute);
            return SendGetAsync<ExternalSupportTicketDto>(route, ct, system);
        }

        public Task<Result<bool>> ReplySupportTicketAsync(Guid id, string reply, CancellationToken ct = default, string? system = null)
        {
            var subRoute = string.Format(_options.Routes.SupportTicketReply, id);
            var route = CombineRoute(_options.Routes.SupportBase, subRoute);
            return SendPostAsync<ReplySupportTicketRequest, bool>(route, new ReplySupportTicketRequest { Reply = reply }, ct, system);
        }

        public Task<Result<bool>> CloseSupportTicketAsync(Guid id, CancellationToken ct = default, string? system = null)
        {
            var subRoute = string.Format(_options.Routes.SupportTicketClose, id);
            var route = CombineRoute(_options.Routes.SupportBase, subRoute);
            return SendPostAsync<object?, bool>(route, null, ct, system);
        }

        public Task<Result<List<ExternalHelpArticleDto>>> GetHelpArticlesAsync(CancellationToken ct = default, string? system = null)
        {
            var subRoute = _options.Routes.HelpArticles;
            var route = CombineRoute(_options.Routes.HelpBase, subRoute);
            return SendGetAsync<List<ExternalHelpArticleDto>>(route, ct, system);
        }

        public Task<Result<List<ExternalFAQDto>>> GetFaqsAsync(CancellationToken ct = default, string? system = null)
        {
            var subRoute = _options.Routes.HelpFaqs;
            var route = CombineRoute(_options.Routes.HelpBase, subRoute);
            return SendGetAsync<List<ExternalFAQDto>>(route, ct, system);
        }

        public Task<Result<List<ExternalCertificationDto>>> GetCertificationsAsync(CancellationToken ct = default, string? system = null)
        {
            var route = _options.Routes.CertificationsBase;
            return SendGetAsync<List<ExternalCertificationDto>>(route, ct, system);
        }

        public Task<Result<ExternalCertificationDto>> GetCertificationByIdAsync(Guid id, CancellationToken ct = default, string? system = null)
        {
            var subRoute = string.Format(_options.Routes.CertificationsGetById, id);
            var route = CombineRoute(_options.Routes.CertificationsBase, subRoute);
            return SendGetAsync<ExternalCertificationDto>(route, ct, system);
        }

        private static string CombineRoute(string baseRoute, string subRoute)
        {
            if (string.IsNullOrWhiteSpace(subRoute)) return baseRoute;
            return $"{baseRoute.TrimEnd('/')}/{subRoute.TrimStart('/')}";
        }

        private static Uri BuildUri(WelcoSystemTarget target, string relativeUrl)
        {
            // Absolute per-system URI keeps the shared HttpClient safe for concurrent
            // tenants. Falls back to the client's BaseAddress when unconfigured (legacy).
            if (!string.IsNullOrWhiteSpace(target.BaseUrl))
                return new Uri(new Uri(target.BaseUrl.TrimEnd('/') + "/"), relativeUrl.TrimStart('/'));
            return new Uri(relativeUrl, UriKind.Relative);
        }

        private async Task<Result<T>> SendGetAsync<T>(string relativeUrl, CancellationToken ct, string? system)
        {
            var target = _resolver.Resolve(system);
            try
            {
                using var req = new HttpRequestMessage(HttpMethod.Get, BuildUri(target, relativeUrl));
                await AttachHeadersAsync(req, target, ct).ConfigureAwait(false);

                var resp = await _http.SendAsync(req, ct);
                return await ParseResponseAsync<T>(resp, relativeUrl, ct);
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
            {
                return HandleException<T>(ex, relativeUrl);
            }
        }

        private async Task<Result<TResponse>> SendPostAsync<TRequest, TResponse>(
            string relativeUrl, TRequest body, CancellationToken ct, string? system)
        {
            var target = _resolver.Resolve(system);
            try
            {
                using var req = new HttpRequestMessage(HttpMethod.Post, BuildUri(target, relativeUrl));
                req.Content = JsonContent.Create(body);
                await AttachHeadersAsync(req, target, ct).ConfigureAwait(false);

                var resp = await _http.SendAsync(req, ct);
                return await ParseResponseAsync<TResponse>(resp, relativeUrl, ct);
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
            {
                return HandleException<TResponse>(ex, relativeUrl);
            }
        }

        private async Task<Result<TResponse>> SendPutAsync<TRequest, TResponse>(
            string relativeUrl, TRequest body, CancellationToken ct, string? system)
        {
            var target = _resolver.Resolve(system);
            try
            {
                using var req = new HttpRequestMessage(HttpMethod.Put, BuildUri(target, relativeUrl));
                req.Content = JsonContent.Create(body);
                await AttachHeadersAsync(req, target, ct).ConfigureAwait(false);

                var resp = await _http.SendAsync(req, ct);
                return await ParseResponseAsync<TResponse>(resp, relativeUrl, ct);
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
            {
                return HandleException<TResponse>(ex, relativeUrl);
            }
        }

        private async Task<Result<T>> ParseResponseAsync<T>(
            HttpResponseMessage resp,
            string url,
            CancellationToken ct)
        {
            if (resp.IsSuccessStatusCode)
            {
                var wrapper = await resp.Content
                    .ReadFromJsonAsync<Result<T>>(cancellationToken: ct)
                    .ConfigureAwait(false);
                if (wrapper != null) return wrapper;
                return Result<T>.ServerError("Empty response from Welco.");
            }

            var raw = await resp.Content.ReadAsStringAsync(ct);
            return Result<T>.Failure($"Welco returned {(int)resp.StatusCode}: {resp.ReasonPhrase}",
                (int)resp.StatusCode);
        }

        private Result<T> HandleException<T>(Exception ex, string url)
        {
            if (ex is TaskCanceledException)
            {
                return Result<T>.Failure(LocalizationKeys.Integration.Timeout, 504);
            }
            return Result<T>.Failure(LocalizationKeys.Integration.Unavailable, 502);
        }

        private async Task AttachHeadersAsync(HttpRequestMessage req, WelcoSystemTarget target, CancellationToken ct)
        {
            // External-auth overhaul: use ONLY the Welco-issued service token.
            // UnauthorizedAccessException and caller-cancellation propagate as before;
            // any other fetch failure also propagates so Send* try/catch maps it
            // to 502/504 via HandleException. No local mint fallback.
            var token = await GetServiceTokenAsync(target, ct).ConfigureAwait(false);

            req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Bearer", token);
            req.Headers.TryAddWithoutValidation("X-Client", "snul");
        }
    }
}
