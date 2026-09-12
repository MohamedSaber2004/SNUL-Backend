using MediatR;
using SNUL.Shared.Common.DTOs.Integration;
using SNUL.Shared.Common.Interfaces;
using SNUL.Shared.Results;

namespace Commerce.Services.API.Features.Integration.Quotes.Commands.UpdateExternalQuoteStatus
{
    public class UpdateExternalQuoteStatusCommand : IRequest<Result<bool>>, IWelcoSystemRequest
    {
        public Guid WelcoQuoteId { get; set; }
        public string Status { get; set; } = null!;
        public string? Notes { get; set; }
        public string? System { get; set; }
    }

    public class UpdateExternalQuoteStatusCommandHandler : IRequestHandler<UpdateExternalQuoteStatusCommand, Result<bool>>
    {
        private readonly IWelcoIntegrationService _welcoService;

        public UpdateExternalQuoteStatusCommandHandler(IWelcoIntegrationService welcoService)
        {
            _welcoService = welcoService;
        }

        public Task<Result<bool>> Handle(UpdateExternalQuoteStatusCommand request, CancellationToken ct)
        {
            var req = new UpdateExternalStatusRequest
            {
                Status = request.Status,
                Notes = request.Notes
            };
            return _welcoService.UpdateQuoteStatusAsync(request.WelcoQuoteId, req, ct, request.System);
        }
    }
}
