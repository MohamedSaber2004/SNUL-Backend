using MediatR;
using SNUL.Shared.Common.DTOs.Integration;
using SNUL.Shared.Common.Interfaces;
using SNUL.Shared.Common.Repositories.Interfaces.Base;
using SNUL.Shared.Domain.Models;
using SNUL.Shared.Results;

namespace Commerce.Services.API.Features.Integration.Providers.Queries.GetExternalProviders
{
    public class GetExternalProvidersQuery : IRequest<Result<List<ExternalProviderDto>>>, IWelcoSystemRequest
    {
        public string? System { get; set; }
    }

    public class GetExternalProvidersQueryHandler : IRequestHandler<GetExternalProvidersQuery, Result<List<ExternalProviderDto>>>
    {
        private readonly IWelcoIntegrationService _welcoService;
        private readonly IWelcoSystemResolver _resolver;
        private readonly IUnitOfWork _unitOfWork;

        public GetExternalProvidersQueryHandler(
            IWelcoIntegrationService welcoService,
            IWelcoSystemResolver resolver,
            IUnitOfWork unitOfWork)
        {
            _welcoService = welcoService;
            _resolver = resolver;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<List<ExternalProviderDto>>> Handle(GetExternalProvidersQuery request, CancellationToken ct)
        {
            var system = _resolver.Normalize(request.System);
            var result = await _welcoService.GetProvidersAsync(ct, system);
            if (!result.IsSuccess || result.Data is null)
                return result;

            // Isolation: only providers mapped to this system are visible. Unmapped are hidden.
            var maps = await _unitOfWork.GetRepository<WelcoProviderMap, Guid>()
                .GetAllListAsync(m => m.System == system, ct);
            var allowed = maps.Select(m => m.WelcoProviderId).ToHashSet();

            return Result<List<ExternalProviderDto>>.Success(
                result.Data.Where(p => allowed.Contains(p.Id)).ToList(),
                result.Message);
        }
    }
}
