using FluentValidation;
using SNUL.Shared.Localization;

namespace UserManagement.Service.API.Features.Zones.Commands.DeleteZone
{
    public class DeleteZoneCommandValidator : AbstractValidator<DeleteZoneCommand>
    {
        public DeleteZoneCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage(LocalizationKeys.Zone.ZoneIdRequired);
        }
    }
}
