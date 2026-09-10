using FluentValidation;
using SNUL.Shared.Localization;

namespace UserManagement.Service.API.Features.Cities.Commands.DeleteCity
{
    public class DeleteCityCommandValidator : AbstractValidator<DeleteCityCommand>
    {
        public DeleteCityCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage(LocalizationKeys.City.CityIdRequired);
        }
    }
}
