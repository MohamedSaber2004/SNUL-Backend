using MediatR;
using SNUL.Shared.Common.DTOs.Integration;
using SNUL.Shared.Common.Interfaces;
using SNUL.Shared.Results;

namespace Commerce.Services.API.Features.Integration.Orders.Commands.UpdateExternalOrderStatus
{
    public class UpdateExternalOrderStatusCommand : IRequest<Result<bool>>, IWelcoSystemRequest
    {
        public Guid WelcoOrderId { get; set; }
        public string Status { get; set; } = null!;
        public string? Notes { get; set; }
        public string? System { get; set; }
    }

    public class UpdateExternalOrderStatusCommandHandler : IRequestHandler<UpdateExternalOrderStatusCommand, Result<bool>>
    {
        private readonly IWelcoIntegrationService _welcoService;

        public UpdateExternalOrderStatusCommandHandler(IWelcoIntegrationService welcoService)
        {
            _welcoService = welcoService;
        }

        public Task<Result<bool>> Handle(UpdateExternalOrderStatusCommand request, CancellationToken ct)
        {
            var req = new UpdateExternalStatusRequest
            {
                Status = request.Status,
                Notes = request.Notes
            };
            return _welcoService.UpdateOrderStatusAsync(request.WelcoOrderId, req, ct, request.System);
        }
    }
}
