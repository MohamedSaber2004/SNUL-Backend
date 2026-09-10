using FluentValidation;
using SNUL.Shared.Localization;

namespace UserManagement.Service.API.Features.Zones.Queries.GetZoneById
{
    public class GetZoneByIdQueryValidator : AbstractValidator<GetZoneByIdQuery>
    {
        public GetZoneByIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage(LocalizationKeys.Zone.ZoneIdRequired);
        }
    }
}
