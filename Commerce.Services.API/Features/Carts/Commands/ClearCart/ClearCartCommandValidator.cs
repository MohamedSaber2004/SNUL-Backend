using FluentValidation;
using SNUL.Shared.Localization;

namespace Commerce.Services.API.Features.Carts.Commands.ClearCart
{
    public class ClearCartCommandValidator : AbstractValidator<ClearCartCommand>
    {
        public ClearCartCommandValidator()
        {
            RuleFor(x => x.CartId).NotEmpty().WithMessage(LocalizationKeys.Cart.CartIdRequired);
        }
    }
}
