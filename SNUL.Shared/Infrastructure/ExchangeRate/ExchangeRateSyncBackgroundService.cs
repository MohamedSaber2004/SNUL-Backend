using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using SNUL.Shared.Common.Interfaces;
using SNUL.Shared.Common.Options;

namespace SNUL.Shared.Infrastructure.ExchangeRate
{
    public class ExchangeRateSyncBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ExchangeRateSettings _settings;

        public ExchangeRateSyncBackgroundService(
            IServiceScopeFactory scopeFactory,
            IOptions<ExchangeRateSettings> options)
        {
            _scopeFactory = scopeFactory;
            _settings = options.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var intervalHours = _settings.SyncIntervalHours > 0 ? _settings.SyncIntervalHours : 24;

            try { await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken); } catch { return; }

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var svc = scope.ServiceProvider.GetRequiredService<IExchangeRateService>();
                    await svc.SyncLatestRatesAsync(stoppingToken);
                }
                catch (Exception)
                {
                }

                try
                {
                    await Task.Delay(TimeSpan.FromHours(intervalHours), stoppingToken);
                }
                catch (TaskCanceledException) { break; }
            }
        }
    }
}
