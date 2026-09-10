using MediatR;
using SNUL.Shared.Common.DTOs.Integration;
using SNUL.Shared.Common.Interfaces;
using SNUL.Shared.Results;

namespace Commerce.Services.API.Features.Integration.Inventory.Commands.ReserveExternalInventory
{
    public class ReserveExternalInventoryCommand : IRequest<Result<InventoryCheckResponse>>
    {
        public List<InventoryCheckItem> Items { get; set; } = new();
    }

    public class ReserveExternalInventoryCommandHandler : IRequestHandler<ReserveExternalInventoryCommand, Result<InventoryCheckResponse>>
    {
        private readonly IWelcoIntegrationService _welcoService;

        public ReserveExternalInventoryCommandHandler(IWelcoIntegrationService welcoService)
        {
            _welcoService = welcoService;
        }

        public Task<Result<InventoryCheckResponse>> Handle(ReserveExternalInventoryCommand request, CancellationToken ct)
            => _welcoService.ReserveInventoryAsync(new InventoryCheckRequest { Items = request.Items }, ct);
    }
}
