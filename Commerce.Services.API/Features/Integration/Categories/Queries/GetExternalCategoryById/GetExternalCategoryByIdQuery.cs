using MediatR;
using SNUL.Shared.Common.DTOs.Integration;
using SNUL.Shared.Common.Interfaces;
using SNUL.Shared.Results;

namespace Commerce.Services.API.Features.Integration.Categories.Queries.GetExternalCategoryById
{
    public class GetExternalCategoryByIdQuery : IRequest<Result<ExternalCategoryDto>>, IWelcoSystemRequest
    {
        public Guid WelcoCategoryId { get; set; }
        public string? System { get; set; }
    }

    public class GetExternalCategoryByIdQueryHandler : IRequestHandler<GetExternalCategoryByIdQuery, Result<ExternalCategoryDto>>
    {
        private readonly IWelcoIntegrationService _welcoService;

        public GetExternalCategoryByIdQueryHandler(IWelcoIntegrationService welcoService)
        {
            _welcoService = welcoService;
        }

        public Task<Result<ExternalCategoryDto>> Handle(GetExternalCategoryByIdQuery request, CancellationToken ct)
            => _welcoService.GetCategoryByIdAsync(request.WelcoCategoryId, ct, request.System);
    }
}
