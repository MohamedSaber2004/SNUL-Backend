using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SNUL.Shared.Common.Options;
using SNUL.Shared.Localization;
using SNUL.Shared.Localization.Interfaces;

namespace SNUL.Shared.Common.Extensions
{
    public static class JwtAuthenticationExtensions
    {
        private const string DefaultFallbackSecret = "V5B?*77+gzD_pk+2!%ORg<i)<D$DH+Xf.nECc?];2l;";

        public static IServiceCollection AddSnulJwtAuthentication(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var jwtSettings = new JwtSettings();
            configuration.GetSection(JwtSettings.SectionName).Bind(jwtSettings);

            var secret = !string.IsNullOrWhiteSpace(jwtSettings.Secret) && jwtSettings.Secret.Length >= 32
                ? jwtSettings.Secret
                : DefaultFallbackSecret;

            var validIssuers = jwtSettings.GetAllValidIssuers().ToList();
            var validAudiences = jwtSettings.GetAllValidAudiences().ToList();

            // NOTE: Must set each default explicitly. AddIdentity() (called before this
            // via AddSnulIdentity) explicitly sets Authenticate/Challenge/Forbid schemes to
            // cookies — the one-arg AddAuthentication(scheme) overload only sets DefaultScheme
            // (fallback) and does NOT override those, which left every API challenging via
            // cookies (401 + Location: /Account/Login) even with a valid Admin JWT.
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
                        ValidateIssuer = validIssuers.Count > 0,
                        ValidIssuers = validIssuers.Count > 0 ? validIssuers : null,
                        ValidateAudience = validAudiences.Count > 0,
                        ValidAudiences = validAudiences.Count > 0 ? validAudiences : null,
                        RequireExpirationTime = true,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.FromMinutes(1)
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnChallenge = async ctx =>
                        {
                            ctx.HandleResponse();
                            ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            ctx.Response.ContentType = "application/json";
                            var loc = ctx.HttpContext.RequestServices.GetService<ILocalizationProvider>();
                            var lang = ctx.Request.Headers["Accept-Language"].FirstOrDefault()?.Split(',')[0].Trim().ToLowerInvariant().StartsWith("ar") == true ? "ar" : "en";
                            var msg = loc?.GetLocalizedString("ExceptionMessages.Unauthorized", lang) ?? "Unauthorized";
                            await ctx.Response.WriteAsJsonAsync(new
                            {
                                isSuccess = false,
                                statusCode = 401,
                                message = msg,
                                errors = new[] { msg },
                                data = (object?)null
                            });
                        }
                    };
                });

            return services;
        }
    }
}
