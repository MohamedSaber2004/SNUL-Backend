using MediatR;
using SNUL.Shared.Common.DTOs.Certifications;
using SNUL.Shared.Results;

namespace Certification.Services.API.Features.Certifications.Queries.GetCertificationById
{
    public class GetCertificationByIdQuery : IRequest<Result<CertificationDto>>
    {
        public Guid Id { get; set; }
    }
}
