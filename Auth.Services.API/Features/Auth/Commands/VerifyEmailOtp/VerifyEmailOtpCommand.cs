using MediatR;
using SNUL.Shared.Common.DTOs.Auth.Responses;
using SNUL.Shared.Results;

namespace Auth.Services.API.Features.Auth.Commands.VerifyEmailOtp
{
    public class VerifyEmailOtpCommand : IRequest<Result<AuthResponseDto>>
    {
        public string Email { get; set; } = null!;
        public string OtpCode { get; set; } = null!;
    }
}
