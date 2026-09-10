using FluentValidation;
using SNUL.Shared.Localization;
namespace UserManagement.Service.API.Features.Companies.Queries.GetCompanyById
{
    public class GetCompanyByIdQueryValidator : AbstractValidator<GetCompanyByIdQuery>
    {
        public GetCompanyByIdQueryValidator() { RuleFor(x => x.Id).NotEmpty().WithMessage(LocalizationKeys.Company.CompanyIdRequired); }
    }
}
