using FluentValidation;
using SNUL.Shared.Localization;

namespace UserManagement.Service.API.Features.Users.Commands.UpdateUser
{
    public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
    {
        public UpdateUserCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage(LocalizationKeys.UserManagement.UserIdRequired);

            RuleFor(x => x.FullName)
                .MaximumLength(150)
                .When(x => !string.IsNullOrEmpty(x.FullName));

            RuleFor(x => x.UserType)
                .IsInEnum().WithMessage(LocalizationKeys.UserManagement.UserTypeRequired)
                .When(x => x.UserType.HasValue);
        }
    }
}
