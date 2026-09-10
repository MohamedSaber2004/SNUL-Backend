using MediatR;
using SNUL.Shared.Common.DTOs.Integration;
using SNUL.Shared.Common.Interfaces;
using SNUL.Shared.Results;

namespace Commerce.Services.API.Features.Integration.Inventory.Queries.CheckExternalInventory
{
    public class CheckExternalInventoryQuery : IRequest<Result<InventoryCheckResponse>>
    {
        public List<InventoryCheckItem> Items { get; set; } = new();
    }

    public class CheckExternalInventoryQueryHandler : IRequestHandler<CheckExternalInventoryQuery, Result<InventoryCheckResponse>>
    {
        private readonly IWelcoIntegrationService _welcoService;

        public CheckExternalInventoryQueryHandler(IWelcoIntegrationService welcoService)
        {
            _welcoService = welcoService;
        }

        public Task<Result<InventoryCheckResponse>> Handle(CheckExternalInventoryQuery request, CancellationToken ct)
            => _welcoService.CheckInventoryAsync(new InventoryCheckRequest { Items = request.Items }, ct);
    }
}
