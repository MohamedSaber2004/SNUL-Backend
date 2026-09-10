using MediatR;
using SNUL.Shared.Common.DTOs.Integration;
using SNUL.Shared.Common.Interfaces;
using SNUL.Shared.Results;

namespace Commerce.Services.API.Features.Integration.Categories.Queries.GetExternalCategories
{
    public class GetExternalCategoriesQuery : IRequest<Result<List<ExternalCategoryDto>>>
    {
    }

    public class GetExternalCategoriesQueryHandler : IRequestHandler<GetExternalCategoriesQuery, Result<List<ExternalCategoryDto>>>
    {
        private readonly IWelcoIntegrationService _welcoService;

        public GetExternalCategoriesQueryHandler(IWelcoIntegrationService welcoService)
        {
            _welcoService = welcoService;
        }

        public Task<Result<List<ExternalCategoryDto>>> Handle(GetExternalCategoriesQuery request, CancellationToken ct)
            => _welcoService.GetCategoriesAsync(ct);
    }
}
