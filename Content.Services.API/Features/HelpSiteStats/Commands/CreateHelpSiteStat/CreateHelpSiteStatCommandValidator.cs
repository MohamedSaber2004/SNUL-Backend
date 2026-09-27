using FluentValidation;
using SNUL.Shared.Localization;

namespace Content.Services.API.Features.HelpSiteStats.Commands.CreateHelpSiteStat
{
    public class CreateHelpSiteStatCommandValidator : AbstractValidator<CreateHelpSiteStatCommand>
    {
        public CreateHelpSiteStatCommandValidator()
        {
            RuleFor(x => x.StatKey).NotEmpty().WithMessage(LocalizationKeys.HelpSiteStat.StatKeyRequired).MaximumLength(100);
            RuleFor(x => x.Value).NotEmpty().WithMessage(LocalizationKeys.HelpSiteStat.ValueRequired).MaximumLength(200);
            RuleFor(x => x.Label).NotEmpty().WithMessage(LocalizationKeys.HelpSiteStat.LabelRequired).MaximumLength(200);
            RuleFor(x => x.LabelAr).MaximumLength(200).When(x => x.LabelAr != null);
        }
    }
}
