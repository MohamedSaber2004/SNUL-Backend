using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SNUL.Shared.Common.DTOs.Integration;
using SNUL.Shared.Common.Interfaces;
using SNUL.Shared.Common.Options;
using SNUL.Shared.Localization;
using SNUL.Shared.Results;

namespace SNUL.Shared.Common.Services
{
    public class WelcoIntegrationService : IWelcoIntegrationService
    {
        private readonly HttpClient _http;
        private readonly WelcoIntegrationOptions _options;
        public WelcoIntegrationService(
            HttpClient http,
            IOptions<WelcoIntegrationOptions> options)
        {
            _http = http;
            _options = options.Value;
        }

        public Task<Result<List<ExternalProviderDto>>> GetProvidersAsync(CancellationToken ct = default)
        {
            var route = _options.Routes.ProvidersBase;
            return SendGetAsync<List<ExternalProviderDto>>(route, ct);
        }

        public Task<Result<List<ExternalProductDto>>> GetProductsAsync(int page = 1, int pageSize = 50, CancellationToken ct = default)
        {
            var route = $"{_options.Routes.ProductsBase}?page={page}&pageSize={pageSize}";
            return SendGetAsync<List<ExternalProductDto>>(route, ct);
        }

        public Task<Result<ExternalProductDto>> GetProductByIdAsync(Guid welcoProductId, CancellationToken ct = default)
        {
            var subRoute = string.Format(_options.Routes.ProductsGetById, welcoProductId);
            var route = CombineRoute(_options.Routes.ProductsBase, subRoute);
            return SendGetAsync<ExternalProductDto>(route, ct);
        }

        public Task<Result<InventoryCheckResponse>> CheckInventoryAsync(InventoryCheckRequest request, CancellationToken ct = default)
        {
            var route = CombineRoute(_options.Routes.InventoryBase, _options.Routes.InventoryCheck);
            return SendPostAsync<InventoryCheckRequest, InventoryCheckResponse>(route, request, ct);
        }

        public Task<Result<InventoryCheckResponse>> ReserveInventoryAsync(InventoryCheckRequest request, CancellationToken ct = default)
        {
            var route = CombineRoute(_options.Routes.InventoryBase, _options.Routes.InventoryReserve);
            return SendPostAsync<InventoryCheckRequest, InventoryCheckResponse>(route, request, ct);
        }

        public Task<Result<ExternalOrderResponse>> CreateOrderAsync(CreateExternalOrderRequest request, CancellationToken ct = default)
        {
            var route = _options.Routes.OrdersBase;
            return SendPostAsync<CreateExternalOrderRequest, ExternalOrderResponse>(route, request, ct);
        }

        public Task<Result<ExternalOrderResponse>> GetOrderByIdAsync(Guid welcoOrderId, CancellationToken ct = default)
        {
            var subRoute = string.Format(_options.Routes.OrdersGetById, welcoOrderId);
            var route = CombineRoute(_options.Routes.OrdersBase, subRoute);
            return SendGetAsync<ExternalOrderResponse>(route, ct);
        }

        public Task<Result<bool>> UpdateOrderStatusAsync(Guid welcoOrderId, UpdateExternalStatusRequest request, CancellationToken ct = default)
        {
            var subRoute = string.Format(_options.Routes.OrdersUpdateStatus, welcoOrderId);
            var route = CombineRoute(_options.Routes.OrdersBase, subRoute);
            return SendPutAsync<UpdateExternalStatusRequest, bool>(route, request, ct);
        }

        public Task<Result<ExternalQuoteResponse>> CreateQuoteAsync(CreateExternalQuoteRequest request, CancellationToken ct = default)
        {
            var route = _options.Routes.QuotesBase;
            return SendPostAsync<CreateExternalQuoteRequest, ExternalQuoteResponse>(route, request, ct);
        }

        public Task<Result<bool>> UpdateQuoteStatusAsync(Guid welcoQuoteId, UpdateExternalStatusRequest request, CancellationToken ct = default)
        {
            var subRoute = string.Format(_options.Routes.QuotesUpdateStatus, welcoQuoteId);
            var route = CombineRoute(_options.Routes.QuotesBase, subRoute);
            return SendPutAsync<UpdateExternalStatusRequest, bool>(route, request, ct);
        }

        public Task<Result<List<ExternalCategoryDto>>> GetCategoriesAsync(CancellationToken ct = default)
        {
            var route = _options.Routes.CategoriesBase;
            return SendGetAsync<List<ExternalCategoryDto>>(route, ct);
        }

        public Task<Result<ExternalCategoryDto>> GetCategoryByIdAsync(Guid welcoCategoryId, CancellationToken ct = default)
        {
            var subRoute = string.Format(_options.Routes.CategoriesGetById, welcoCategoryId);
            var route = CombineRoute(_options.Routes.CategoriesBase, subRoute);
            return SendGetAsync<ExternalCategoryDto>(route, ct);
        }

        public Task<Result<DistributorApplicationDto>> SubmitDistributorApplicationAsync(ApplyDistributorRequest request, CancellationToken ct = default)
        {
            var route = CombineRoute(_options.Routes.DistributorsBase, _options.Routes.DistributorsApply);
            return SendPostAsync<ApplyDistributorRequest, DistributorApplicationDto>(route, request, ct);
        }

        public Task<Result<List<DistributorApplicationDto>>> GetDistributorApplicationsAsync(CancellationToken ct = default)
        {
            var route = _options.Routes.DistributorsBase;
            return SendGetAsync<List<DistributorApplicationDto>>(route, ct);
        }

        public Task<Result<DistributorApplicationDto>> GetDistributorApplicationByIdAsync(Guid id, CancellationToken ct = default)
        {
            var subRoute = string.Format(_options.Routes.DistributorsGetById, id);
            var route = CombineRoute(_options.Routes.DistributorsBase, subRoute);
            return SendGetAsync<DistributorApplicationDto>(route, ct);
        }

        public Task<Result<bool>> ApproveDistributorApplicationAsync(Guid id, CancellationToken ct = default)
        {
            var subRoute = string.Format(_options.Routes.DistributorsApprove, id);
            var route = CombineRoute(_options.Routes.DistributorsBase, subRoute);
            return SendPutAsync<object?, bool>(route, null, ct);
        }

        public Task<Result<bool>> RejectDistributorApplicationAsync(Guid id, string? reason = null, CancellationToken ct = default)
        {
            var subRoute = string.Format(_options.Routes.DistributorsReject, id);
            var route = CombineRoute(_options.Routes.DistributorsBase, subRoute);
            return SendPutAsync<object, bool>(route, new { Reason = reason }, ct);
        }

        public Task<Result<List<ExternalSupportTicketDto>>> GetSupportTicketsAsync(string? status = null, CancellationToken ct = default)
        {
            var subRoute = _options.Routes.SupportTickets;
            if (!string.IsNullOrWhiteSpace(status))
                subRoute += $"?status={Uri.EscapeDataString(status)}";
            var route = CombineRoute(_options.Routes.SupportBase, subRoute);
            return SendGetAsync<List<ExternalSupportTicketDto>>(route, ct);
        }

        public Task<Result<ExternalSupportTicketDto>> GetSupportTicketByIdAsync(Guid id, CancellationToken ct = default)
        {
            var subRoute = string.Format(_options.Routes.SupportTicketById, id);
            var route = CombineRoute(_options.Routes.SupportBase, subRoute);
            return SendGetAsync<ExternalSupportTicketDto>(route, ct);
        }

        public Task<Result<bool>> ReplySupportTicketAsync(Guid id, string reply, CancellationToken ct = default)
        {
            var subRoute = string.Format(_options.Routes.SupportTicketReply, id);
            var route = CombineRoute(_options.Routes.SupportBase, subRoute);
            return SendPostAsync<ReplySupportTicketRequest, bool>(route, new ReplySupportTicketRequest { Reply = reply }, ct);
        }

        public Task<Result<bool>> CloseSupportTicketAsync(Guid id, CancellationToken ct = default)
        {
            var subRoute = string.Format(_options.Routes.SupportTicketClose, id);
            var route = CombineRoute(_options.Routes.SupportBase, subRoute);
            return SendPostAsync<object?, bool>(route, null, ct);
        }

        public Task<Result<List<ExternalHelpArticleDto>>> GetHelpArticlesAsync(CancellationToken ct = default)
        {
            var subRoute = _options.Routes.HelpArticles;
            var route = CombineRoute(_options.Routes.HelpBase, subRoute);
            return SendGetAsync<List<ExternalHelpArticleDto>>(route, ct);
        }

        public Task<Result<List<ExternalFAQDto>>> GetFaqsAsync(CancellationToken ct = default)
        {
            var subRoute = _options.Routes.HelpFaqs;
            var route = CombineRoute(_options.Routes.HelpBase, subRoute);
            return SendGetAsync<List<ExternalFAQDto>>(route, ct);
        }

        public Task<Result<List<ExternalCertificationDto>>> GetCertificationsAsync(CancellationToken ct = default)
        {
            var route = _options.Routes.CertificationsBase;
            return SendGetAsync<List<ExternalCertificationDto>>(route, ct);
        }

        public Task<Result<ExternalCertificationDto>> GetCertificationByIdAsync(Guid id, CancellationToken ct = default)
        {
            var subRoute = string.Format(_options.Routes.CertificationsGetById, id);
            var route = CombineRoute(_options.Routes.CertificationsBase, subRoute);
            return SendGetAsync<ExternalCertificationDto>(route, ct);
        }

        private static string CombineRoute(string baseRoute, string subRoute)
        {
            if (string.IsNullOrWhiteSpace(subRoute)) return baseRoute;
            return $"{baseRoute.TrimEnd('/')}/{subRoute.TrimStart('/')}";
        }

        private async Task<Result<T>> SendGetAsync<T>(string relativeUrl, CancellationToken ct)
        {
            try
            {
                using var req = new HttpRequestMessage(HttpMethod.Get, relativeUrl);
                AttachHeaders(req);

                var resp = await _http.SendAsync(req, ct);
                return await ParseResponseAsync<T>(resp, relativeUrl, ct);
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
            {
                return HandleException<T>(ex, relativeUrl);
            }
        }

        private async Task<Result<TResponse>> SendPostAsync<TRequest, TResponse>(
            string relativeUrl, TRequest body, CancellationToken ct)
        {
            try
            {
                using var req = new HttpRequestMessage(HttpMethod.Post, relativeUrl);
                req.Content = JsonContent.Create(body);
                AttachHeaders(req);

                var resp = await _http.SendAsync(req, ct);
                return await ParseResponseAsync<TResponse>(resp, relativeUrl, ct);
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
            {
                return HandleException<TResponse>(ex, relativeUrl);
            }
        }

        private async Task<Result<TResponse>> SendPutAsync<TRequest, TResponse>(
            string relativeUrl, TRequest body, CancellationToken ct)
        {
            try
            {
                using var req = new HttpRequestMessage(HttpMethod.Put, relativeUrl);
                req.Content = JsonContent.Create(body);
                AttachHeaders(req);

                var resp = await _http.SendAsync(req, ct);
                return await ParseResponseAsync<TResponse>(resp, relativeUrl, ct);
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

        private void AttachHeaders(HttpRequestMessage req)
        {
            req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Bearer", GenerateServiceJwt());
            req.Headers.TryAddWithoutValidation("X-Client", "snul");
        }

        private string GenerateServiceJwt()
        {
            if (string.IsNullOrWhiteSpace(_options.ServiceSecret) || _options.ServiceSecret.Length < 32)
                throw new InvalidOperationException("WelcoIntegration:ServiceSecret must be at least 32 characters. Configure via environment variables.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.ServiceSecret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, "snul-integration"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("service", "snul"),
                new Claim("market", "Egypt")
            };

            var token = new JwtSecurityToken(
                issuer: _options.ServiceIssuer,
                audience: _options.ServiceAudience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(_options.TokenExpiryMinutes),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
