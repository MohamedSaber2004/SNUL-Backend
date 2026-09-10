using MediatR;
using SNUL.Shared.Common.DTOs.Certifications;
using SNUL.Shared.Results;

namespace Certification.Services.API.Features.Certifications.Queries.GetCertifications
{
    public class GetCertificationsQuery : IRequest<PaginatedResult<CertificationDto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public bool? IsActive { get; set; }
    }
}
