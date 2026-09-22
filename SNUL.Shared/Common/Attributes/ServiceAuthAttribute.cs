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
            if (context.ActionDescriptor.EndpointMetadata.Any(em => em is IAllowAnonymous))
            {
                return Task.CompletedTask;
            }

            var settings = context.HttpContext.RequestServices
                .GetRequiredService<IOptions<JwtSettings>>().Value;

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

            if (!ValidateServiceToken(token, settings, logger))
            {
                Deny(context);
                return Task.CompletedTask;
            }

            logger.LogInformation("[ServiceAuth] Authenticated service request for Path={Path}", context.HttpContext.Request.Path);

            return Task.CompletedTask;
        }

        private static bool ValidateServiceToken(
            string token,
            JwtSettings settings,
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

                var clientId = jwt.Claims.FirstOrDefault(c => c.Type == "client_id")?.Value;
                if (!string.IsNullOrWhiteSpace(clientId))
                {
                    logger.LogWarning("[ServiceAuth] Service token with client_id is not supported.");
                    return false;
                }

                if (string.IsNullOrWhiteSpace(settings.Secret) || settings.Secret.Length < 32)
                {
                    logger.LogError("[ServiceAuth] JwtSettings.Secret is not configured (min 32 chars).");
                    return false;
                }

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Secret));

                handler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = !string.IsNullOrWhiteSpace(settings.Issuer),
                    ValidIssuer = settings.Issuer,
                    ValidateAudience = !string.IsNullOrWhiteSpace(settings.Audience),
                    ValidAudience = settings.Audience,
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

            var message = Localize(LocalizationKeys.ExceptionMessages.Unauthorized);
            var response = Result<object?>.Unauthorized(message, new List<string> { message });

            context.Result = new JsonResult(response)
            {
                StatusCode = StatusCodes.Status401Unauthorized
            };
        }
    }
}