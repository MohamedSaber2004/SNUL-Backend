using FluentValidation;
using SNUL.Shared.Localization;

namespace UserManagement.Service.API.Features.DistributorApplications.Commands.ApproveDistributorApplication
{
    public class ApproveDistributorApplicationCommandValidator : AbstractValidator<ApproveDistributorApplicationCommand>
    {
        public ApproveDistributorApplicationCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage(LocalizationKeys.DistributorApplication.ApplicationIdRequired);
        }
    }
}
