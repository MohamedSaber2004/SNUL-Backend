using FluentValidation;
using SNUL.Shared.Localization;

namespace Content.Services.API.Features.SiteLogo.Commands.UpsertSiteLogo
{
    public class UpsertSiteLogoCommandValidator : AbstractValidator<UpsertSiteLogoCommand>
    {
        public UpsertSiteLogoCommandValidator()
        {
            RuleFor(x => x.LogoUrl)
                .MaximumLength(2048);

            RuleFor(x => x.AltText)
                .MaximumLength(200);
        }
    }
}
