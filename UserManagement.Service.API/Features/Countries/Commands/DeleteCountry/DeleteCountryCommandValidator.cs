using FluentValidation;
using SNUL.Shared.Localization;

namespace UserManagement.Service.API.Features.Countries.Commands.DeleteCountry
{
    public class DeleteCountryCommandValidator : AbstractValidator<DeleteCountryCommand>
    {
        public DeleteCountryCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage(LocalizationKeys.Country.CountryIdRequired);
        }
    }
}
