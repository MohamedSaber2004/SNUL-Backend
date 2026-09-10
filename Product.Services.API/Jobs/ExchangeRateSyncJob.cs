using SNUL.Shared.Common.Interfaces;

namespace Product.Services.API.Jobs
{
    public class ExchangeRateSyncJob
    {
        private readonly IExchangeRateService _exchangeRateService;

        public ExchangeRateSyncJob(IExchangeRateService exchangeRateService)
        {
            _exchangeRateService = exchangeRateService;
        }

        public async Task ExecuteAsync()
        {
            var result = await _exchangeRateService.SyncLatestRatesAsync(CancellationToken.None);

            if (!result.Success)
            {
                throw new InvalidOperationException($"Exchange rate sync failed: {result.ErrorMessage}");
            }
        }
    }
}
