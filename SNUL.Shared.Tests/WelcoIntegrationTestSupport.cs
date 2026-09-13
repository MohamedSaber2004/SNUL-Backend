using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;
using SNUL.Shared.Common.DTOs.Integration;
using SNUL.Shared.Results;

namespace SNUL.Shared.Tests;

internal static class WelcoIntegrationTestSupport
{
    internal static TokenValidationParameters ValidationParameters(string secret, string issuer, string audience) =>
        new()
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
        };

    internal sealed class CaptureHandler : HttpMessageHandler
    {
        public string? CapturedAuthorizationParameter { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            CapturedAuthorizationParameter = request.Headers.Authorization?.Parameter;
            var payload = JsonSerializer.Serialize(Result<List<ExternalProviderDto>>.Success(new()));
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(payload, Encoding.UTF8, "application/json"),
            });
        }
    }
}
