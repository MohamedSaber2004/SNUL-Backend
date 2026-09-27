using FluentValidation;
using SNUL.Shared.Localization;

namespace Content.Services.API.Features.HelpSiteStats.Commands.UpdateHelpSiteStat
{
    public class UpdateHelpSiteStatCommandValidator : AbstractValidator<UpdateHelpSiteStatCommand>
    {
        public UpdateHelpSiteStatCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage(LocalizationKeys.HelpSiteStat.HelpSiteStatIdRequired);
            RuleFor(x => x.Value).NotEmpty().WithMessage(LocalizationKeys.HelpSiteStat.ValueRequired).MaximumLength(200);
            RuleFor(x => x.Label).NotEmpty().WithMessage(LocalizationKeys.HelpSiteStat.LabelRequired).MaximumLength(200);
            RuleFor(x => x.LabelAr).MaximumLength(200).When(x => x.LabelAr != null);
        }
    }
}
