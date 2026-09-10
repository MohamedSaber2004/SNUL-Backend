using FluentValidation;
using SNUL.Shared.Localization;

namespace Certification.Services.API.Features.Certifications.Queries.GetCertificationById
{
    public class GetCertificationByIdQueryValidator : AbstractValidator<GetCertificationByIdQuery>
    {
        public GetCertificationByIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage(LocalizationKeys.Certification.CertificationIdRequired);
        }
    }
}
