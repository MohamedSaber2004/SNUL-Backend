using MediatR;
using SNUL.Shared.Common.DTOs.Auth.Responses;
using SNUL.Shared.Results;

namespace Auth.Services.API.Features.Auth.Commands.Login
{
    public class LoginCommand : IRequest<Result<AuthResponseDto>>
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
