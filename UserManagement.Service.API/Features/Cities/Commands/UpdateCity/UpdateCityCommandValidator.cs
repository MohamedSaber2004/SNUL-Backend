using FluentValidation;
using SNUL.Shared.Localization;

namespace UserManagement.Service.API.Features.Cities.Commands.UpdateCity
{
    public class UpdateCityCommandValidator : AbstractValidator<UpdateCityCommand>
    {
        public UpdateCityCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage(LocalizationKeys.City.CityIdRequired);

            RuleFor(x => x.NameEn)
                .MaximumLength(150)
                .When(x => !string.IsNullOrEmpty(x.NameEn));

            RuleFor(x => x.NameAr)
                .MaximumLength(150)
                .When(x => !string.IsNullOrEmpty(x.NameAr));
        }
    }
}
