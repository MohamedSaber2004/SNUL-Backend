using MediatR;
using SNUL.Shared.Common.DTOs.Integration;
using SNUL.Shared.Common.Interfaces;
using SNUL.Shared.Results;

namespace Commerce.Services.API.Features.Integration.Quotes.Commands.CreateExternalQuote
{
    public class CreateExternalQuoteCommand : IRequest<Result<ExternalQuoteResponse>>
    {
        public string? SourceMarket { get; set; } = "Egypt";
        public List<ExternalQuoteItemRequest> Items { get; set; } = new();
    }

    public class CreateExternalQuoteCommandHandler : IRequestHandler<CreateExternalQuoteCommand, Result<ExternalQuoteResponse>>
    {
        private readonly IWelcoIntegrationService _welcoService;

        public CreateExternalQuoteCommandHandler(IWelcoIntegrationService welcoService)
        {
            _welcoService = welcoService;
        }

        public Task<Result<ExternalQuoteResponse>> Handle(CreateExternalQuoteCommand request, CancellationToken ct)
        {
            var req = new CreateExternalQuoteRequest
            {
                SourceMarket = request.SourceMarket,
                Items = request.Items
            };
            return _welcoService.CreateQuoteAsync(req, ct);
        }
    }
}
