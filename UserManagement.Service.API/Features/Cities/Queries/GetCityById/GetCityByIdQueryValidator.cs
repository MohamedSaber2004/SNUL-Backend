using FluentValidation;
using SNUL.Shared.Localization;

namespace UserManagement.Service.API.Features.Cities.Queries.GetCityById
{
    public class GetCityByIdQueryValidator : AbstractValidator<GetCityByIdQuery>
    {
        public GetCityByIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage(LocalizationKeys.City.CityIdRequired);
        }
    }
}
