using MediatR;
using SNUL.Shared.Common.DTOs.Integration;
using SNUL.Shared.Common.Interfaces;
using SNUL.Shared.Results;

namespace Commerce.Services.API.Features.Integration.Providers.Queries.GetExternalProviders
{
    public class GetExternalProvidersQuery : IRequest<Result<List<ExternalProviderDto>>>
    {
    }

    public class GetExternalProvidersQueryHandler : IRequestHandler<GetExternalProvidersQuery, Result<List<ExternalProviderDto>>>
    {
        private readonly IWelcoIntegrationService _welcoService;

        public GetExternalProvidersQueryHandler(IWelcoIntegrationService welcoService)
        {
            _welcoService = welcoService;
        }

        public Task<Result<List<ExternalProviderDto>>> Handle(GetExternalProvidersQuery request, CancellationToken ct)
            => _welcoService.GetProvidersAsync(ct);
    }
}
