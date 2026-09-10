using MediatR;
using SNUL.Shared.Common.DTOs.Integration;
using SNUL.Shared.Common.Interfaces;
using SNUL.Shared.Results;

namespace Commerce.Services.API.Features.Integration.Certifications
{
    public class GetExternalCertificationsQuery : IRequest<Result<List<ExternalCertificationDto>>> { }

    public class GetExternalCertificationsQueryHandler : IRequestHandler<GetExternalCertificationsQuery, Result<List<ExternalCertificationDto>>>
    {
        private readonly IWelcoIntegrationService _welcoService;
        public GetExternalCertificationsQueryHandler(IWelcoIntegrationService welcoService) => _welcoService = welcoService;

        public Task<Result<List<ExternalCertificationDto>>> Handle(GetExternalCertificationsQuery request, CancellationToken cancellationToken)
            => _welcoService.GetCertificationsAsync(cancellationToken);
    }

    public class GetExternalCertificationByIdQuery : IRequest<Result<ExternalCertificationDto>>
    {
        public Guid Id { get; set; }
    }

    public class GetExternalCertificationByIdQueryHandler : IRequestHandler<GetExternalCertificationByIdQuery, Result<ExternalCertificationDto>>
    {
        private readonly IWelcoIntegrationService _welcoService;
        public GetExternalCertificationByIdQueryHandler(IWelcoIntegrationService welcoService) => _welcoService = welcoService;

        public Task<Result<ExternalCertificationDto>> Handle(GetExternalCertificationByIdQuery request, CancellationToken cancellationToken)
            => _welcoService.GetCertificationByIdAsync(request.Id, cancellationToken);
    }
}
