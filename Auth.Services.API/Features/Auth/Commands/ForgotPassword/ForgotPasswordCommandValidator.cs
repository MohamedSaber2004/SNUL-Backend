using FluentValidation;
using Microsoft.AspNetCore.Identity;
using SNUL.Shared.Domain.Models;
using SNUL.Shared.Localization;
using SNUL.Shared.Results;

namespace Auth.Services.API.Features.Auth.Commands.ForgotPassword
{
    public class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
    {
        public ForgotPasswordCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(LocalizationKeys.Auth.EmailRequired)
                .EmailAddress().WithMessage(LocalizationKeys.Auth.EmailInvalid);
        }
    }
}
