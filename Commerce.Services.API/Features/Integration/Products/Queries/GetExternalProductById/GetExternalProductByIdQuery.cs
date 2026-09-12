using MediatR;
using SNUL.Shared.Common.DTOs.Integration;
using SNUL.Shared.Common.Interfaces;
using SNUL.Shared.Results;

namespace Commerce.Services.API.Features.Integration.Products.Queries.GetExternalProductById
{
    public class GetExternalProductByIdQuery : IRequest<Result<ExternalProductDto>>, IWelcoSystemRequest
    {
        public Guid WelcoProductId { get; set; }
        public string? System { get; set; }
    }

    public class GetExternalProductByIdQueryHandler : IRequestHandler<GetExternalProductByIdQuery, Result<ExternalProductDto>>
    {
        private readonly IWelcoIntegrationService _welcoService;

        public GetExternalProductByIdQueryHandler(IWelcoIntegrationService welcoService)
        {
            _welcoService = welcoService;
        }

        public Task<Result<ExternalProductDto>> Handle(GetExternalProductByIdQuery request, CancellationToken ct)
            => _welcoService.GetProductByIdAsync(request.WelcoProductId, ct, request.System);
    }
}
