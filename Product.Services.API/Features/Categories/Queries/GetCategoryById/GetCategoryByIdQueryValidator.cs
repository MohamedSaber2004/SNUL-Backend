using FluentValidation;
using SNUL.Shared.Localization;

namespace Product.Services.API.Features.Categories.Queries.GetCategoryById
{
    public class GetCategoryByIdQueryValidator : AbstractValidator<GetCategoryByIdQuery>
    {
        public GetCategoryByIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage(LocalizationKeys.Category.CategoryIdRequired);
        }
    }
}
