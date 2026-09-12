using MediatR;
using SNUL.Shared.Common.DTOs.Integration;
using SNUL.Shared.Common.Interfaces;
using SNUL.Shared.Results;

namespace Commerce.Services.API.Features.Integration.Products.Queries.GetExternalProducts
{
    public class GetExternalProductsQuery : IRequest<Result<List<ExternalProductDto>>>, IWelcoSystemRequest
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;
        public string? System { get; set; }
    }

    public class GetExternalProductsQueryHandler : IRequestHandler<GetExternalProductsQuery, Result<List<ExternalProductDto>>>
    {
        private readonly IWelcoIntegrationService _welcoService;

        public GetExternalProductsQueryHandler(IWelcoIntegrationService welcoService)
        {
            _welcoService = welcoService;
        }

        public Task<Result<List<ExternalProductDto>>> Handle(GetExternalProductsQuery request, CancellationToken ct)
            => _welcoService.GetProductsAsync(request.Page, request.PageSize, ct, request.System);
    }
}
