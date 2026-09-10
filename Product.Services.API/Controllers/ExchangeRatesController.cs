using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Product.Services.API.ProductRoutes;
using SNUL.Shared.Common.Attributes;
using SNUL.Shared.Common.DTOs.Products;
using SNUL.Shared.Common.Interfaces;
using SNUL.Shared.Controllers;
using SNUL.Shared.Domain.Models;
using SNUL.Shared.Enums;
using SNUL.Shared.Localization;
using SNUL.Shared.Results;

namespace Product.Services.API.Controllers
{
    [Route(ProductApiRoutes.ExchangeRates.Base)]
    public class ExchangeRatesController : AppControllerBase
    {
        private readonly IExchangeRateService _service;

        public ExchangeRatesController(IMediator mediator, IExchangeRateService service) : base(mediator) => _service = service;

                [HttpGet]
        [Route(ProductApiRoutes.ExchangeRates.Latest)]
        [AllowAnonymous]
        public async Task<IActionResult> GetLatest(CancellationToken ct)
        {
            var rates = await _service.GetLatestRatesAsync("USD", ct);
            return ToActionResult(Result<IReadOnlyCollection<ExchangeRateDto>>.Success(rates.ToList(), LocalizationKeys.ExchangeRate.ListFetched));
        }

                [HttpGet]
        [Route(ProductApiRoutes.ExchangeRates.LatestByBase)]
        [AllowAnonymous]
        public async Task<IActionResult> GetLatestByBase([FromRoute] string baseCurrency, CancellationToken ct)
        {
            var rates = await _service.GetLatestRatesAsync(baseCurrency, ct);
            return ToActionResult(Result<IReadOnlyCollection<ExchangeRateDto>>.Success(rates.ToList(), LocalizationKeys.ExchangeRate.ListFetched));
        }

                [HttpGet]
        [Route(ProductApiRoutes.ExchangeRates.Pair)]
        [AllowAnonymous]
        public async Task<IActionResult> GetPair([FromRoute] string from, [FromRoute] string to, CancellationToken ct)
        {
            var rate = await _service.GetLatestRateAsync(from, to, ct);
            if (rate == null) return ToActionResult(Result<ExchangeRateDto>.NotFound(LocalizationKeys.ExchangeRate.NotFound));
            return ToActionResult(Result<ExchangeRateDto>.Success(rate, LocalizationKeys.ExchangeRate.Fetched));
        }

                [HttpGet]
        [Route(ProductApiRoutes.ExchangeRates.Convert)]
        [AllowAnonymous]
        public async Task<IActionResult> Convert([FromQuery] string from, [FromQuery] string to, [FromQuery] decimal amount, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(to))
                return ToActionResult(Result<ConversionResultDto>.BadRequest(LocalizationKeys.ExchangeRate.FromAndToRequired));
            if (amount < 0)
                return ToActionResult(Result<ConversionResultDto>.BadRequest(LocalizationKeys.ExchangeRate.AmountNonNegative));

            var result = await _service.ConvertWithDetailsAsync(amount, from, to, ct);
            return ToActionResult(Result<ConversionResultDto>.Success(result, LocalizationKeys.ExchangeRate.Fetched));
        }

                [HttpGet]
        [Route(ProductApiRoutes.ExchangeRates.History)]
        [AllowAnonymous]
        public async Task<IActionResult> GetHistory([FromRoute] string baseCurrency, [FromRoute] string date, CancellationToken ct)
        {
            if (!DateOnly.TryParse(date, out var d))
                return ToActionResult(Result<IReadOnlyCollection<ExchangeRateDto>>.BadRequest(LocalizationKeys.ExchangeRate.InvalidDateFormat));

            var rates = await _service.GetHistoricalRatesAsync(baseCurrency, d, ct);
            return ToActionResult(Result<IReadOnlyCollection<ExchangeRateDto>>.Success(rates.ToList(), LocalizationKeys.ExchangeRate.ListFetched));
        }

                [HttpPost]
        [Route(ProductApiRoutes.ExchangeRates.Sync)]
        [RoleAuthorize(UserType.Admin)]
        public async Task<IActionResult> Sync(CancellationToken ct)
        {
            var result = await _service.SyncLatestRatesAsync(ct);
            if (!result.Success) return ToActionResult(Result<ExchangeRateSyncResult>.Failure(LocalizationKeys.ExchangeRate.SyncFailed, 502));
            return ToActionResult(Result<ExchangeRateSyncResult>.Success(result, LocalizationKeys.ExchangeRate.SyncSuccess));
        }

        [HttpPost]
        [Route(ProductApiRoutes.ExchangeRates.SyncHistory)]
        [RoleAuthorize(UserType.Admin)]
        public async Task<IActionResult> SyncHistory([FromRoute] string date, CancellationToken ct)
        {
            if (!DateOnly.TryParse(date, out var d))
                return ToActionResult(Result<ExchangeRateSyncResult>.BadRequest(LocalizationKeys.ExchangeRate.InvalidDateFormat));
            var result = await _service.SyncHistoricalRatesAsync(d, ct);
            if (!result.Success) return ToActionResult(Result<ExchangeRateSyncResult>.Failure(LocalizationKeys.ExchangeRate.SyncFailed, 502));
            return ToActionResult(Result<ExchangeRateSyncResult>.Success(result, LocalizationKeys.ExchangeRate.SyncSuccess));
        }

                [HttpGet]
        [Route(ProductApiRoutes.ExchangeRates.SyncLogs)]
        [AllowAnonymous]
        public async Task<IActionResult> GetSyncLogs([FromQuery] int take = 10, CancellationToken ct = default)
        {
            var logs = await _service.GetSyncLogsAsync(take, ct);
            return ToActionResult(Result<IReadOnlyCollection<ExchangeRateSyncLog>>.Success(logs, LocalizationKeys.ExchangeRate.ListFetched));
        }
    }
}
