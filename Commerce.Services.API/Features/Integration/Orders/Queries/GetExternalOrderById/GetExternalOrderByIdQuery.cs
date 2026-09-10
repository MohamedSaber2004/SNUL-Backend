using MediatR;
using SNUL.Shared.Common.DTOs.Integration;
using SNUL.Shared.Common.Interfaces;
using SNUL.Shared.Results;

namespace Commerce.Services.API.Features.Integration.Orders.Queries.GetExternalOrderById
{
    public class GetExternalOrderByIdQuery : IRequest<Result<ExternalOrderResponse>>
    {
        public Guid WelcoOrderId { get; set; }
    }

    public class GetExternalOrderByIdQueryHandler : IRequestHandler<GetExternalOrderByIdQuery, Result<ExternalOrderResponse>>
    {
        private readonly IWelcoIntegrationService _welcoService;

        public GetExternalOrderByIdQueryHandler(IWelcoIntegrationService welcoService)
        {
            _welcoService = welcoService;
        }

        public Task<Result<ExternalOrderResponse>> Handle(GetExternalOrderByIdQuery request, CancellationToken ct)
            => _welcoService.GetOrderByIdAsync(request.WelcoOrderId, ct);
    }
}
