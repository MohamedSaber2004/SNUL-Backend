using FluentValidation;
using SNUL.Shared.Localization;

namespace UserManagement.Service.API.Features.Users.Commands.DeleteUser
{
    public class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
    {
        public DeleteUserCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage(LocalizationKeys.UserManagement.UserIdRequired);
        }
    }
}
