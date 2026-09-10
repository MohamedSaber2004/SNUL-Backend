using MediatR;
using SNUL.Shared.Common.DTOs.Integration;
using SNUL.Shared.Common.Interfaces;
using SNUL.Shared.Results;

namespace Commerce.Services.API.Features.Integration.Distributors
{
    public class ApplyExternalDistributorCommand : IRequest<Result<DistributorApplicationDto>>
    {
        public ApplyDistributorRequest Request { get; set; } = new();
    }

    public class ApplyExternalDistributorCommandHandler : IRequestHandler<ApplyExternalDistributorCommand, Result<DistributorApplicationDto>>
    {
        private readonly IWelcoIntegrationService _welcoService;
        public ApplyExternalDistributorCommandHandler(IWelcoIntegrationService welcoService) => _welcoService = welcoService;

        public Task<Result<DistributorApplicationDto>> Handle(ApplyExternalDistributorCommand request, CancellationToken cancellationToken)
            => _welcoService.SubmitDistributorApplicationAsync(request.Request, cancellationToken);
    }

    public class GetExternalDistributorsQuery : IRequest<Result<List<DistributorApplicationDto>>> { }

    public class GetExternalDistributorsQueryHandler : IRequestHandler<GetExternalDistributorsQuery, Result<List<DistributorApplicationDto>>>
    {
        private readonly IWelcoIntegrationService _welcoService;
        public GetExternalDistributorsQueryHandler(IWelcoIntegrationService welcoService) => _welcoService = welcoService;

        public Task<Result<List<DistributorApplicationDto>>> Handle(GetExternalDistributorsQuery request, CancellationToken cancellationToken)
            => _welcoService.GetDistributorApplicationsAsync(cancellationToken);
    }

    public class GetExternalDistributorByIdQuery : IRequest<Result<DistributorApplicationDto>>
    {
        public Guid Id { get; set; }
    }

    public class GetExternalDistributorByIdQueryHandler : IRequestHandler<GetExternalDistributorByIdQuery, Result<DistributorApplicationDto>>
    {
        private readonly IWelcoIntegrationService _welcoService;
        public GetExternalDistributorByIdQueryHandler(IWelcoIntegrationService welcoService) => _welcoService = welcoService;

        public Task<Result<DistributorApplicationDto>> Handle(GetExternalDistributorByIdQuery request, CancellationToken cancellationToken)
            => _welcoService.GetDistributorApplicationByIdAsync(request.Id, cancellationToken);
    }

    public class ApproveExternalDistributorCommand : IRequest<Result<bool>>
    {
        public Guid Id { get; set; }
    }

    public class ApproveExternalDistributorCommandHandler : IRequestHandler<ApproveExternalDistributorCommand, Result<bool>>
    {
        private readonly IWelcoIntegrationService _welcoService;
        public ApproveExternalDistributorCommandHandler(IWelcoIntegrationService welcoService) => _welcoService = welcoService;

        public Task<Result<bool>> Handle(ApproveExternalDistributorCommand request, CancellationToken cancellationToken)
            => _welcoService.ApproveDistributorApplicationAsync(request.Id, cancellationToken);
    }

    public class RejectExternalDistributorCommand : IRequest<Result<bool>>
    {
        public Guid Id { get; set; }
        public string? Reason { get; set; }
    }

    public class RejectExternalDistributorCommandHandler : IRequestHandler<RejectExternalDistributorCommand, Result<bool>>
    {
        private readonly IWelcoIntegrationService _welcoService;
        public RejectExternalDistributorCommandHandler(IWelcoIntegrationService welcoService) => _welcoService = welcoService;

        public Task<Result<bool>> Handle(RejectExternalDistributorCommand request, CancellationToken cancellationToken)
            => _welcoService.RejectDistributorApplicationAsync(request.Id, request.Reason, cancellationToken);
    }
}
