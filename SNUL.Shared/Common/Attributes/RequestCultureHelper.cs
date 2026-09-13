using Microsoft.AspNetCore.Http;
using SNUL.Shared.Enums;

namespace SNUL.Shared.Common.Attributes
{
    /// <summary>
    /// Single shared request-culture helper for authorization filters.
    /// Keeps header/query fallback semantics identical across filters.
    /// </summary>
    internal static class RequestCultureHelper
    {
        internal static string GetRequestCulture(HttpContext? context)
        {
            var req = context?.Request;
            if (req != null)
            {
                var headers = req.Headers;
                var hCulture = headers["Accept-Language"].FirstOrDefault()
                               ?? headers["Language"].FirstOrDefault()
                               ?? headers["language"].FirstOrDefault()
                               ?? headers["Culture"].FirstOrDefault()
                               ?? headers["Lang"].FirstOrDefault();

                if (!string.IsNullOrWhiteSpace(hCulture))
                {
                    return AppLanguageExtensions.FromCode(hCulture).ToCode();
                }

                var qCulture = req.Query["culture"].FirstOrDefault()
                               ?? req.Query["lang"].FirstOrDefault()
                               ?? req.Query["language"].FirstOrDefault();

                if (!string.IsNullOrWhiteSpace(qCulture))
                {
                    return AppLanguageExtensions.FromCode(qCulture).ToCode();
                }
            }

            var current = System.Globalization.CultureInfo.CurrentUICulture?.Name;
            return AppLanguageExtensions.FromCode(current).ToCode();
        }
    }
}
