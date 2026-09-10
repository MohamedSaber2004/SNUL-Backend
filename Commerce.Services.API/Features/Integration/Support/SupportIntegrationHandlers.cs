using MediatR;
using SNUL.Shared.Common.DTOs.Integration;
using SNUL.Shared.Common.Interfaces;
using SNUL.Shared.Results;

namespace Commerce.Services.API.Features.Integration.Support
{
    public class GetExternalSupportTicketsQuery : IRequest<Result<List<ExternalSupportTicketDto>>>
    {
        public string? Status { get; set; }
    }

    public class GetExternalSupportTicketsQueryHandler : IRequestHandler<GetExternalSupportTicketsQuery, Result<List<ExternalSupportTicketDto>>>
    {
        private readonly IWelcoIntegrationService _welcoService;
        public GetExternalSupportTicketsQueryHandler(IWelcoIntegrationService welcoService) => _welcoService = welcoService;

        public Task<Result<List<ExternalSupportTicketDto>>> Handle(GetExternalSupportTicketsQuery request, CancellationToken cancellationToken)
            => _welcoService.GetSupportTicketsAsync(request.Status, cancellationToken);
    }

    public class GetExternalSupportTicketByIdQuery : IRequest<Result<ExternalSupportTicketDto>>
    {
        public Guid Id { get; set; }
    }

    public class GetExternalSupportTicketByIdQueryHandler : IRequestHandler<GetExternalSupportTicketByIdQuery, Result<ExternalSupportTicketDto>>
    {
        private readonly IWelcoIntegrationService _welcoService;
        public GetExternalSupportTicketByIdQueryHandler(IWelcoIntegrationService welcoService) => _welcoService = welcoService;

        public Task<Result<ExternalSupportTicketDto>> Handle(GetExternalSupportTicketByIdQuery request, CancellationToken cancellationToken)
            => _welcoService.GetSupportTicketByIdAsync(request.Id, cancellationToken);
    }

    public class ReplyExternalSupportTicketCommand : IRequest<Result<bool>>
    {
        public Guid Id { get; set; }
        public string Reply { get; set; } = null!;
    }

    public class ReplyExternalSupportTicketCommandHandler : IRequestHandler<ReplyExternalSupportTicketCommand, Result<bool>>
    {
        private readonly IWelcoIntegrationService _welcoService;
        public ReplyExternalSupportTicketCommandHandler(IWelcoIntegrationService welcoService) => _welcoService = welcoService;

        public Task<Result<bool>> Handle(ReplyExternalSupportTicketCommand request, CancellationToken cancellationToken)
            => _welcoService.ReplySupportTicketAsync(request.Id, request.Reply, cancellationToken);
    }

    public class CloseExternalSupportTicketCommand : IRequest<Result<bool>>
    {
        public Guid Id { get; set; }
    }

    public class CloseExternalSupportTicketCommandHandler : IRequestHandler<CloseExternalSupportTicketCommand, Result<bool>>
    {
        private readonly IWelcoIntegrationService _welcoService;
        public CloseExternalSupportTicketCommandHandler(IWelcoIntegrationService welcoService) => _welcoService = welcoService;

        public Task<Result<bool>> Handle(CloseExternalSupportTicketCommand request, CancellationToken cancellationToken)
            => _welcoService.CloseSupportTicketAsync(request.Id, cancellationToken);
    }
}
