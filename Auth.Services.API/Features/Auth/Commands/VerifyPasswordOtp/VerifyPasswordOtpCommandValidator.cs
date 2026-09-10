using FluentValidation;
using Microsoft.AspNetCore.Identity;
using SNUL.Shared.Domain.Models;
using SNUL.Shared.Localization;
using SNUL.Shared.Results;

namespace Auth.Services.API.Features.Auth.Commands.VerifyPasswordOtp
{
    public class VerifyPasswordOtpCommandValidator : AbstractValidator<VerifyPasswordOtpCommand>
    {
        public VerifyPasswordOtpCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(LocalizationKeys.Auth.EmailRequired)
                .EmailAddress().WithMessage(LocalizationKeys.Auth.EmailInvalid);

            RuleFor(x => x.OtpCode)
                .NotEmpty().WithMessage(LocalizationKeys.Auth.OtpCodeRequired)
                .Length(6).WithMessage(LocalizationKeys.Auth.OtpCodeFormat);
        }
    }
}
