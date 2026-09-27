using MediatR;
using SNUL.Shared.Common.DTOs.Content;
using SNUL.Shared.Results;

namespace Content.Services.API.Features.HelpSiteStats.Queries.GetHelpSiteStatByKey
{
    public class GetHelpSiteStatByKeyQuery : IRequest<Result<HelpSiteStatDto>>
    {
        public string Key { get; set; } = string.Empty;
    }
}
