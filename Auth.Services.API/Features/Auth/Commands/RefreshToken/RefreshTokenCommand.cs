using MediatR;
using SNUL.Shared.Common.DTOs.Auth.Responses;
using SNUL.Shared.Results;

namespace Auth.Services.API.Features.Auth.Commands.RefreshToken
{
    public class RefreshTokenCommand : IRequest<Result<AuthResponseDto>>
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
}
