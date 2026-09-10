using MediatR;
using Microsoft.EntityFrameworkCore;
using SNUL.Shared.Common.DTOs.Content;
using SNUL.Shared.Domain.Models;
using SNUL.Shared.Localization;
using SNUL.Shared.Persistance;
using SNUL.Shared.Results;

namespace Content.Services.API.Features.TradeShows.Queries.GetTradeShows
{
    public class GetTradeShowsQueryHandler : IRequestHandler<GetTradeShowsQuery, Result<List<TradeShowEventDto>>>
    {
        private readonly SnulDbContext _db;
        public GetTradeShowsQueryHandler(SnulDbContext db) => _db = db;

        public async Task<Result<List<TradeShowEventDto>>> Handle(GetTradeShowsQuery request, CancellationToken ct)
        {
            var q = _db.TradeShowEvents.AsNoTracking().Where(x => !x.IsDeleted);
            if (request.UpcomingOnly == true) q = q.Where(x => x.StartDate >= DateTime.UtcNow.Date);
            var list = await q.OrderBy(x => x.StartDate).Select(x => new TradeShowEventDto
            {
                Id = x.Id,
                Name = x.Name,
                Location = x.Location,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                CreatedAt = x.CreatedAt
            }).ToListAsync(ct);
            return Result<List<TradeShowEventDto>>.Success(list, LocalizationKeys.TradeShow.ListFetched);
        }
    }
}
