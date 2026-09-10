using FluentValidation;
using SNUL.Shared.Localization;

namespace UserManagement.Service.API.Features.Zones.Commands.CreateZone
{
    public class CreateZoneCommandValidator : AbstractValidator<CreateZoneCommand>
    {
        public CreateZoneCommandValidator()
        {
            RuleFor(x => x.CityId)
                .NotEmpty().WithMessage(LocalizationKeys.Zone.CityIdRequired);

            RuleFor(x => x.NameEn)
                .NotEmpty().WithMessage(LocalizationKeys.Zone.NameEnRequired)
                .MaximumLength(150);

            RuleFor(x => x.NameAr)
                .NotEmpty().WithMessage(LocalizationKeys.Zone.NameArRequired)
                .MaximumLength(150);
        }
    }
}
