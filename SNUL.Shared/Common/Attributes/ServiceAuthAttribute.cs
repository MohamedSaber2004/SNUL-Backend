using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SNUL.Shared.Common.Options;
using SNUL.Shared.Localization;
using SNUL.Shared.Localization.Interfaces;
using SNUL.Shared.Results;

namespace SNUL.Shared.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ServiceAuthAttribute : Attribute, IAsyncAuthorizationFilter
    {
        public Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            // Mirrors RoleAuthorize: [AllowAnonymous] bypasses service auth.
            // Verified: IntegrationController (the sole [ServiceAuth] usage) has no
            // [AllowAnonymous], so no behavior change there; authorization-filter
            // ordering only moves this check earlier (before action filters/model
            // binding), which is the correct phase for auth.
            if (context.ActionDescriptor.EndpointMetadata.Any(em => em is IAllowAnonymous))
            {
                return Task.CompletedTask;
            }

            var options = context.HttpContext.RequestServices
                .GetRequiredService<IOptions<WelcoIntegrationOptions>>().Value;

            var logger = context.HttpContext.RequestServices
                .GetRequiredService<ILoggerFactory>()
                .CreateLogger<ServiceAuthAttribute>();

            var authHeader = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            var token = authHeader?.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase) == true
                ? authHeader["Bearer ".Length..].Trim()
                : null;

            if (string.IsNullOrWhiteSpace(token))
            {
                logger.LogWarning("[ServiceAuth] Missing token for Path={Path}", context.HttpContext.Request.Path);
                Deny(context);
                return Task.CompletedTask;
            }

            if (!ValidateServiceToken(token, options, logger))
            {
                Deny(context);
                return Task.CompletedTask;
            }

            logger.LogInformation("[ServiceAuth] Authenticated service request for Path={Path}", context.HttpContext.Request.Path);

            return Task.CompletedTask;
        }

        private static bool ValidateServiceToken(
            string token,
            WelcoIntegrationOptions options,
            ILogger logger)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();

                JwtSecurityToken jwt;
                try
                {
                    jwt = handler.ReadJwtToken(token);
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "[ServiceAuth] Unable to read service token.");
                    return false;
                }

                // No user-JWT path, no legacy bypass: client_id is mandatory.
                var clientId = jwt.Claims.FirstOrDefault(c => c.Type == "client_id")?.Value;
                if (string.IsNullOrWhiteSpace(clientId))
                {
                    logger.LogWarning("[ServiceAuth] Service token without client_id.");
                    return false;
                }

                // Single shared path: key-only canonical resolution (see
                // WelcoIntegrationCredentials for the documented choice/invariant).
                if (!WelcoIntegrationCredentials.TryResolveByClientId(
                    options, clientId, out var secret, out var expectedIssuer, out var expectedAudience))
                {
                    logger.LogWarning("[ServiceAuth] Unknown integration client ClientId={ClientId}.", clientId);
                    return false;
                }

                // Fail closed: empty/short secret or empty issuer/audience is
                // misconfiguration and must 401, never disable validation.
                // Expected values keep today's snul-integration/welco-integration
                // semantics (defaults with per-system overrides).
                if (string.IsNullOrWhiteSpace(secret) || secret.Length < 32)
                {
                    logger.LogWarning("[ServiceAuth] Misconfigured secret for integration client ClientId={ClientId} (min 32 chars).", clientId);
                    return false;
                }

                if (string.IsNullOrWhiteSpace(expectedIssuer) || string.IsNullOrWhiteSpace(expectedAudience))
                {
                    logger.LogWarning("[ServiceAuth] Misconfigured issuer/audience for integration client ClientId={ClientId}.", clientId);
                    return false;
                }

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

                handler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = true,
                    ValidIssuer = expectedIssuer,
                    ValidateAudience = true,
                    ValidAudience = expectedAudience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(2)
                }, out _);

                return true;
            }
            catch (SecurityTokenExpiredException)
            {
                logger.LogWarning("[ServiceAuth] Expired service token.");
                return false;
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "[ServiceAuth] Token validation failed.");
                return false;
            }
        }

        private static void Deny(AuthorizationFilterContext context)
        {
            var localizationProvider = context.HttpContext.RequestServices.GetService<ILocalizationProvider>();
            var culture = RequestCultureHelper.GetRequestCulture(context.HttpContext);

            string Localize(string key)
            {
                if (localizationProvider == null || string.IsNullOrWhiteSpace(key))
                    return key;
                return localizationProvider.GetLocalizedString(key, culture);
            }

            // Mirror RoleAuthorize 401 contract: errors echoes the localized message.
            var message = Localize(LocalizationKeys.ExceptionMessages.Unauthorized);
            var response = Result<object?>.Unauthorized(message, new List<string> { message });

            context.Result = new JsonResult(response)
            {
                StatusCode = StatusCodes.Status401Unauthorized
            };
        }
    }
}
