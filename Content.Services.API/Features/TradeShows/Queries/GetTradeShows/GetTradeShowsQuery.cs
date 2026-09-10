using MediatR;
using SNUL.Shared.Common.DTOs.Content;
using SNUL.Shared.Results;

namespace Content.Services.API.Features.TradeShows.Queries.GetTradeShows
{
    public class GetTradeShowsQuery : IRequest<Result<List<TradeShowEventDto>>>
    {
        public bool? UpcomingOnly { get; set; }
    }
}
