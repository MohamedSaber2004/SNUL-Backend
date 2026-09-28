using MediatR;
using SNUL.Shared.Common.DTOs.Content;
using SNUL.Shared.Results;

namespace Content.Services.API.Features.SiteLogo.Commands.UpsertSiteLogo
{
    public class UpsertSiteLogoCommand : IRequest<Result<SiteLogoDto>>
    {
        public string LogoUrl { get; set; } = string.Empty;
        public string AltText { get; set; } = "SNUL";
    }
}
