using MediatR;
using SNUL.Shared.Common.DTOs.Content;
using SNUL.Shared.Results;

namespace Content.Services.API.Features.HelpSiteStats.Queries.GetHelpSiteStats
{
    public class GetHelpSiteStatsQuery : IRequest<Result<List<HelpSiteStatDto>>>
    {
    }
}
