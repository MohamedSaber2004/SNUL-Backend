namespace SNUL.Gateway.API.Services
{
    public class OpenApiCacheWarmer : BackgroundService
    {
        private readonly OpenApiAggregatorService _aggregator;

        public OpenApiCacheWarmer(OpenApiAggregatorService aggregator)
        {
            _aggregator = aggregator;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                return;
            }

            try
            {
                await _aggregator.WarmUpAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
            }
            catch (Exception)
            {
            }
        }
    }
}
