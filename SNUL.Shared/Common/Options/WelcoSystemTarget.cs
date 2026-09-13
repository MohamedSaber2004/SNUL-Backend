namespace SNUL.Shared.Common.Options
{
    public sealed class WelcoSystemTarget
    {
        public string BaseUrl { get; set; } = string.Empty;
        public string ClientId { get; set; } = "snul";
        public string ClientSecret { get; set; } = string.Empty;
        public string ServiceSecret { get; set; } = string.Empty;
        public string ServiceIssuer { get; set; } = "snul-integration";
        public string ServiceAudience { get; set; } = "welco-integration";
        public string Market { get; set; } = "Egypt";
        public int TimeoutSeconds { get; set; } = 30;
    }
}
