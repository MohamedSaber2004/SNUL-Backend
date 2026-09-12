namespace Commerce.Services.API.Features.Integration
{
    /// <summary>
    /// Marks an integration request as tenant-aware. The dashboard discriminator
    /// ("snul" | "welo", one market each) is set by the controller from
    /// ?system= / X-System and flows to <c>IWelcoIntegrationService</c>.
    /// Null means the configured default system (backward compatible).
    /// </summary>
    public interface IWelcoSystemRequest
    {
        string? System { get; set; }
    }
}
