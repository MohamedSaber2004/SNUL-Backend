using Content.Services.API.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SNUL.Shared.Common.DTOs.Content;
using SNUL.Shared.Common.Repositories.Interfaces.Base;
using SNUL.Shared.Localization;
using SNUL.Shared.Results;
using HelpSiteStatEntity = SNUL.Shared.Domain.Models.HelpSiteStat;

namespace Content.Services.API.Features.HelpSiteStats.Queries.GetHelpSiteStatByKey
{
    public class GetHelpSiteStatByKeyQueryHandler : IRequestHandler<GetHelpSiteStatByKeyQuery, Result<HelpSiteStatDto>>
    {
        private readonly IUnitOfWork _uow;
        public GetHelpSiteStatByKeyQueryHandler(IUnitOfWork uow) => _uow = uow;

        public async Task<Result<HelpSiteStatDto>> Handle(GetHelpSiteStatByKeyQuery request, CancellationToken cancellationToken)
        {
            var key = request.Key.Trim().ToLowerInvariant();
            var repo = _uow.GetRepository<HelpSiteStatEntity, Guid>();

            // Keys are the frontend contract, so match them case-insensitively.
            // IsVisible is honoured here too: this endpoint is public, and returning
            // a withdrawn claim through the single-key route would defeat the point
            // of the flag.
            var dto = await repo.GetAll(s => !s.IsDeleted && s.IsVisible && s.StatKey.ToLower() == key)
                .Select(ContentDtoMapper.HelpSiteStatProjection)
                .FirstOrDefaultAsync(cancellationToken);

            if (dto == null)
                return Result<HelpSiteStatDto>.NotFound(LocalizationKeys.HelpSiteStat.NotFound);

            return Result<HelpSiteStatDto>.Success(dto, LocalizationKeys.HelpSiteStat.Fetched);
        }
    }
}
