using FluentValidation;
using SNUL.Shared.Localization;

namespace UserManagement.Service.API.Features.DistributorApplications.Commands.RejectDistributorApplication
{
    public class RejectDistributorApplicationCommandValidator : AbstractValidator<RejectDistributorApplicationCommand>
    {
        public RejectDistributorApplicationCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage(LocalizationKeys.DistributorApplication.ApplicationIdRequired);
        }
    }
}
