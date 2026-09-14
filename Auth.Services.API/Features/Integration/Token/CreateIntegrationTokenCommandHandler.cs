using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using MediatR;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SNUL.Shared.Common.Options;
using SNUL.Shared.Localization;
using SNUL.Shared.Results;

namespace Auth.Services.API.Features.Integration.Token
{
    public class CreateIntegrationTokenCommandHandler : IRequestHandler<CreateIntegrationTokenCommand, Result<IntegrationTokenResponse>>
    {
        public const int TokenLifetimeMinutes = 55;
        public const int ExpiresInSeconds = 3300;

        private readonly WelcoIntegrationOptions _options;
        public CreateIntegrationTokenCommandHandler(
            IOptions<WelcoIntegrationOptions> options)
        {
            _options = options.Value;
        }

        public Task<Result<IntegrationTokenResponse>> Handle(CreateIntegrationTokenCommand request, CancellationToken cancellationToken)
        {
            var clientId = (request.ClientId ?? string.Empty).Trim();

            if (!WelcoIntegrationCredentials.TryResolveByClientId(
                _options, clientId, out var secret, out var issuer, out var audience))
            {
                return Task.FromResult(Unauthorized());
            }

            if (string.IsNullOrWhiteSpace(secret) || secret.Length < 32
                || string.IsNullOrWhiteSpace(issuer) || string.IsNullOrWhiteSpace(audience))
            {
                return Task.FromResult(Unauthorized());
            }

            var providedSecret = request.ClientSecret ?? string.Empty;
            if (!CryptographicOperations.FixedTimeEquals(
                    Encoding.UTF8.GetBytes(secret),
                    Encoding.UTF8.GetBytes(providedSecret)))
            {
                return Task.FromResult(Unauthorized());
            }

            var now = DateTime.UtcNow;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.WriteToken(handler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = issuer,
                Audience = audience,
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, "snul-integration"),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("client_id", clientId),
                    new Claim("service", "snul"),
                    new Claim("market", ResolveMarket(clientId)),
                }),
                NotBefore = now,
                Expires = now.AddMinutes(TokenLifetimeMinutes),
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            }));

            return Task.FromResult(Result<IntegrationTokenResponse>.Success(
                new IntegrationTokenResponse
                {
                    AccessToken = jwt,
                    ExpiresIn = ExpiresInSeconds,
                    TokenType = "Bearer"
                }));
        }

        private string ResolveMarket(string clientId)
        {
            if (_options.Systems.TryGetValue(clientId, out var target)
                && target is not null
                && !string.IsNullOrWhiteSpace(target.Market))
            {
                return target.Market;
            }

            return "Egypt";
        }

        private static Result<IntegrationTokenResponse> Unauthorized()
        {
            return Result<IntegrationTokenResponse>.Unauthorized(
                LocalizationKeys.Integration.InvalidCredientials,
                new List<string> { LocalizationKeys.Integration.InvalidCredientials });
        }
    }
}
