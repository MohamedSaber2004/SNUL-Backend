using FluentValidation;
using SNUL.Shared.Localization;

namespace Commerce.Services.API.Features.Carts.Queries.GetCartById
{
    public class GetCartByIdQueryValidator : AbstractValidator<GetCartByIdQuery>
    {
        public GetCartByIdQueryValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage(LocalizationKeys.Cart.CartIdRequired);
        }
    }
}
