using Microsoft.AspNetCore.Http;

namespace SNUL.Shared.Common.Extensions
{
    /// <summary>
    /// Reads the dashboard tenant discriminator. Header wins over query string
    /// so links stay shareable (?system=) while integrations use X-System.
    /// </summary>
    public static class WelcoSystemRequestExtensions
    {
        public static string? GetWelcoSystem(this HttpRequest request)
        {
            var header = request.Headers["X-System"].FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(header))
                return header;

            return request.Query["system"].FirstOrDefault();
        }
    }
}
