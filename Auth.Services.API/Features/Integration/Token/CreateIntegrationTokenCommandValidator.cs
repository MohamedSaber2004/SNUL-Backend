using FluentValidation;
using SNUL.Shared.Localization;

namespace Auth.Services.API.Features.Integration.Token
{
    public class CreateIntegrationTokenCommandValidator : AbstractValidator<CreateIntegrationTokenCommand>
    {
        public CreateIntegrationTokenCommandValidator()
        {
            RuleFor(x => x.ClientId)
                .NotEmpty().WithMessage(LocalizationKeys.Integration.ClientIdRequired);

            RuleFor(x => x.ClientSecret)
                .NotEmpty().WithMessage(LocalizationKeys.Integration.ClientSecretRequired);
        }
    }
}
