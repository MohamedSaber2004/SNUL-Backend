using FluentValidation;
using SNUL.Shared.Localization;

namespace Product.Services.API.Features.Currencies.Queries.GetCurrencyById
{
    public class GetCurrencyByIdQueryValidator : AbstractValidator<GetCurrencyByIdQuery>
    {
        public GetCurrencyByIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage(LocalizationKeys.Currency.CurrencyIdRequired);
        }
    }
}
