using FluentValidation;
using SNUL.Shared.Localization;

namespace UserManagement.Service.API.Features.Users.Queries.GetUserById
{
    public class GetUserByIdQueryValidator : AbstractValidator<GetUserByIdQuery>
    {
        public GetUserByIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage(LocalizationKeys.UserManagement.UserIdRequired);
        }
    }
}
