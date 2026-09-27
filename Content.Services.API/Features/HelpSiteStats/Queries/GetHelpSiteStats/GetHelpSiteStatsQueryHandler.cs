using Content.Services.API.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SNUL.Shared.Common.DTOs.Content;
using SNUL.Shared.Common.Repositories.Interfaces.Base;
using SNUL.Shared.Localization;
using SNUL.Shared.Results;
using HelpSiteStatEntity = SNUL.Shared.Domain.Models.HelpSiteStat;

namespace Content.Services.API.Features.HelpSiteStats.Queries.GetHelpSiteStats
{
    public class GetHelpSiteStatsQueryHandler : IRequestHandler<GetHelpSiteStatsQuery, Result<List<HelpSiteStatDto>>>
    {
        private readonly IUnitOfWork _uow;
        public GetHelpSiteStatsQueryHandler(IUnitOfWork uow) => _uow = uow;

        public async Task<Result<List<HelpSiteStatDto>>> Handle(GetHelpSiteStatsQuery request, CancellationToken cancellationToken)
        {
            var repo = _uow.GetRepository<HelpSiteStatEntity, Guid>();

            // IsVisible is the withdrawal switch: an admin sets it false to take a
            // claim down without deleting the row. Nothing is seeded, so an empty
            // list is the correct answer for an unconfigured database, not an error.
            var list = await repo.GetAll(s => !s.IsDeleted && s.IsVisible)
                .AsNoTracking()
                .OrderBy(s => s.SortOrder)
                .ThenBy(s => s.StatKey)
                .Select(ContentDtoMapper.HelpSiteStatProjection)
                .ToListAsync(cancellationToken);

            return Result<List<HelpSiteStatDto>>.Success(list, LocalizationKeys.HelpSiteStat.ListFetched);
        }
    }
}
