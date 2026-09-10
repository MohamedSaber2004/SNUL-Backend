using MediatR;
using SNUL.Shared.Common.DTOs.Certifications;
using SNUL.Shared.Results;

namespace Certification.Services.API.Features.Certifications.Queries.ShowCertification
{
    public class ShowCertificationQuery : IRequest<Result<CertificationDto>>
    {
        public Guid Id { get; set; }
    }
}
