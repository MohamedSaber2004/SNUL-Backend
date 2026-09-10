using FluentValidation;
using SNUL.Shared.Localization;
namespace UserManagement.Service.API.Features.Companies.Commands.DeleteCompany
{
    public class DeleteCompanyCommandValidator : AbstractValidator<DeleteCompanyCommand>
    {
        public DeleteCompanyCommandValidator() { RuleFor(x => x.Id).NotEmpty().WithMessage(LocalizationKeys.Company.CompanyIdRequired); }
    }
}
