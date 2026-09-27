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
            try
            {
                var rates = await _service.GetLatestRatesAsync("USD", ct);
                return ToActionResult(Result<IReadOnlyCollection<ExchangeRateDto>>.Success(rates.ToList(), LocalizationKeys.ExchangeRate.ListFetched));
            }
            catch (Exception ex)
            {
                return ToActionResult(Result<IReadOnlyCollection<ExchangeRateDto>>.Failure(ex.Message));
            }
        }

        [HttpGet]
        [Route(ProductApiRoutes.ExchangeRates.LatestByBase)]
        [AllowAnonymous]
        public async Task<IActionResult> GetLatestByBase([FromRoute] string baseCurrency, CancellationToken ct)
        {
            try
            {
                var rates = await _service.GetLatestRatesAsync(baseCurrency, ct);
                return ToActionResult(Result<IReadOnlyCollection<ExchangeRateDto>>.Success(rates.ToList(), LocalizationKeys.ExchangeRate.ListFetched));
            }
            catch (Exception ex)
            {
                return ToActionResult(Result<IReadOnlyCollection<ExchangeRateDto>>.Failure(ex.Message));
            }
        }

        [HttpGet]
        [Route(ProductApiRoutes.ExchangeRates.Pair)]
        [AllowAnonymous]
        public async Task<IActionResult> GetPair([FromRoute] string from, [FromRoute] string to, CancellationToken ct)
        {
            try
            {
                var rate = await _service.GetLatestRateAsync(from, to, ct);
                if (rate == null) return ToActionResult(Result<ExchangeRateDto>.NotFound(LocalizationKeys.ExchangeRate.NotFound));
                return ToActionResult(Result<ExchangeRateDto>.Success(rate, LocalizationKeys.ExchangeRate.Fetched));
            }
            catch (Exception ex)
            {
                return ToActionResult(Result<ExchangeRateDto>.Failure(ex.Message));
            }
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

            try
            {
                var result = await _service.ConvertWithDetailsAsync(amount, from, to, ct);
                return ToActionResult(Result<ConversionResultDto>.Success(result, LocalizationKeys.ExchangeRate.Fetched));
            }
            catch (Exception ex)
            {
                return ToActionResult(Result<ConversionResultDto>.Failure(ex.Message));
            }
        }

        /// <summary>
        /// Converts a whole cart in a single round-trip. Each line is converted
        /// independently (mixed-currency carts are supported) and the safety margin
        /// is applied to every non-identity line before the subtotal is summed.
        /// </summary>
        [HttpPost]
        [Route(ProductApiRoutes.ExchangeRates.CartTotal)]
        [AllowAnonymous]
        public async Task<IActionResult> ConvertCartTotal([FromBody] ConvertCartTotalRequest request, CancellationToken ct)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.ToCurrency))
                return ToActionResult(Result<CartTotalResultDto>.BadRequest("toCurrency is required"));
            if (request.Lines == null || request.Lines.Count == 0)
                return ToActionResult(Result<CartTotalResultDto>.BadRequest("lines are required"));
            if (request.Lines.Count > 200)
                return ToActionResult(Result<CartTotalResultDto>.BadRequest("too many lines (max 200)"));

            try
            {
                var result = await _service.ConvertCartTotalAsync(request, ct);
                return ToActionResult(Result<CartTotalResultDto>.Success(result, LocalizationKeys.ExchangeRate.Fetched));
            }
            catch (Exception ex)
            {
                return ToActionResult(Result<CartTotalResultDto>.Failure(ex.Message));
            }
        }
    }
}
