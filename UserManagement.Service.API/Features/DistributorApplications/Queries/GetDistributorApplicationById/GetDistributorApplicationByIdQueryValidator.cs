using FluentValidation;
using SNUL.Shared.Localization;

namespace UserManagement.Service.API.Features.DistributorApplications.Queries.GetDistributorApplicationById
{
    public class GetDistributorApplicationByIdQueryValidator : AbstractValidator<GetDistributorApplicationByIdQuery>
    {
        public GetDistributorApplicationByIdQueryValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage(LocalizationKeys.DistributorApplication.ApplicationIdRequired);
        }
    }
}
