using MediatR;
using SNUL.Shared.Common.DTOs.Integration;
using SNUL.Shared.Common.Interfaces;
using SNUL.Shared.Results;

namespace Commerce.Services.API.Features.Integration.Orders.Commands.CreateExternalOrder
{
    public class CreateExternalOrderCommand : IRequest<Result<ExternalOrderResponse>>
    {
        public string? SourceMarket { get; set; } = "Egypt";
        public Guid? ExternalCustomerId { get; set; }
        public Guid? CurrencyId { get; set; }
        public decimal TotalAmount { get; set; }
        public List<ExternalOrderItemRequest> Items { get; set; } = new();
    }

    public class CreateExternalOrderCommandHandler : IRequestHandler<CreateExternalOrderCommand, Result<ExternalOrderResponse>>
    {
        private readonly IWelcoIntegrationService _welcoService;

        public CreateExternalOrderCommandHandler(IWelcoIntegrationService welcoService)
        {
            _welcoService = welcoService;
        }

        public Task<Result<ExternalOrderResponse>> Handle(CreateExternalOrderCommand request, CancellationToken ct)
        {
            var req = new CreateExternalOrderRequest
            {
                SourceMarket = request.SourceMarket,
                ExternalCustomerId = request.ExternalCustomerId,
                CurrencyId = request.CurrencyId,
                TotalAmount = request.TotalAmount,
                Items = request.Items
            };
            return _welcoService.CreateOrderAsync(req, ct);
        }
    }
}
