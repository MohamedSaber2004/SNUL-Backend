using MediatR;
using SNUL.Shared.Common.DTOs.Integration;
using SNUL.Shared.Common.Interfaces;
using SNUL.Shared.Results;

namespace Commerce.Services.API.Features.Integration.Quotes.Commands.CreateExternalQuote
{
    public class CreateExternalQuoteCommand : IRequest<Result<ExternalQuoteResponse>>, IWelcoSystemRequest
    {
        public string? SourceMarket { get; set; }
        public List<ExternalQuoteItemRequest> Items { get; set; } = new();
        public string? System { get; set; }
    }

    public class CreateExternalQuoteCommandHandler : IRequestHandler<CreateExternalQuoteCommand, Result<ExternalQuoteResponse>>
    {
        private readonly IWelcoIntegrationService _welcoService;
        private readonly IWelcoSystemResolver _resolver;

        public CreateExternalQuoteCommandHandler(IWelcoIntegrationService welcoService, IWelcoSystemResolver resolver)
        {
            _welcoService = welcoService;
            _resolver = resolver;
        }

        public Task<Result<ExternalQuoteResponse>> Handle(CreateExternalQuoteCommand request, CancellationToken ct)
        {
            var req = new CreateExternalQuoteRequest
            {
                SourceMarket = string.IsNullOrWhiteSpace(request.SourceMarket)
                    ? _resolver.Resolve(request.System).Market
                    : request.SourceMarket,
                Items = request.Items
            };
            return _welcoService.CreateQuoteAsync(req, ct, request.System);
        }
    }
}
