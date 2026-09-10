using MediatR;
using SNUL.Shared.Common.DTOs.Integration;
using SNUL.Shared.Common.Interfaces;
using SNUL.Shared.Results;

namespace Commerce.Services.API.Features.Integration.Help
{
    public class GetExternalHelpArticlesQuery : IRequest<Result<List<ExternalHelpArticleDto>>> { }

    public class GetExternalHelpArticlesQueryHandler : IRequestHandler<GetExternalHelpArticlesQuery, Result<List<ExternalHelpArticleDto>>>
    {
        private readonly IWelcoIntegrationService _welcoService;
        public GetExternalHelpArticlesQueryHandler(IWelcoIntegrationService welcoService) => _welcoService = welcoService;

        public Task<Result<List<ExternalHelpArticleDto>>> Handle(GetExternalHelpArticlesQuery request, CancellationToken cancellationToken)
            => _welcoService.GetHelpArticlesAsync(cancellationToken);
    }

    public class GetExternalFaqsQuery : IRequest<Result<List<ExternalFAQDto>>> { }

    public class GetExternalFaqsQueryHandler : IRequestHandler<GetExternalFaqsQuery, Result<List<ExternalFAQDto>>>
    {
        private readonly IWelcoIntegrationService _welcoService;
        public GetExternalFaqsQueryHandler(IWelcoIntegrationService welcoService) => _welcoService = welcoService;

        public Task<Result<List<ExternalFAQDto>>> Handle(GetExternalFaqsQuery request, CancellationToken cancellationToken)
            => _welcoService.GetFaqsAsync(cancellationToken);
    }
}
