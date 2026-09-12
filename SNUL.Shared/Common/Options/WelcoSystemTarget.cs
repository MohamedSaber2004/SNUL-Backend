namespace SNUL.Shared.Common.Options
{
    /// <summary>
    /// Per-system Welco endpoint settings. One entry per isolated system (e.g. "snul", "welo").
    /// System == market (one market each): <see cref="Market"/> travels as the "market" claim
    /// and the default SourceMarket for orders/quotes.
    /// </summary>
    public sealed class WelcoSystemTarget
    {
        public string BaseUrl { get; set; } = string.Empty;
        public string ServiceSecret { get; set; } = string.Empty;
        public string ServiceIssuer { get; set; } = "snul-integration";
        public string ServiceAudience { get; set; } = "welco-integration";
        public string Market { get; set; } = "Egypt";
        public int TimeoutSeconds { get; set; } = 30;
    }
}
