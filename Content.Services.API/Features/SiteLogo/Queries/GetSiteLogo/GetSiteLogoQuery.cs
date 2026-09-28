using MediatR;
using SNUL.Shared.Common.DTOs.Content;
using SNUL.Shared.Results;

namespace Content.Services.API.Features.SiteLogo.Queries.GetSiteLogo
{
    public class GetSiteLogoQuery : IRequest<Result<SiteLogoDto>> { }
}
