using Content.Services.API.ContentRoutes;
using Content.Services.API.Features.SiteLogo.Commands.UpsertSiteLogo;
using Content.Services.API.Features.SiteLogo.Queries.GetSiteLogo;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SNUL.Shared.Common.Attributes;
using SNUL.Shared.Controllers;
using SNUL.Shared.Enums;

namespace Content.Services.API.Controllers
{
    [RoleAuthorize]
    [Route(ContentApiRoutes.SiteLogo.Base)]
    public class SiteLogoController : AppControllerBase
    {
        public SiteLogoController(IMediator mediator) : base(mediator) { }

        [HttpGet]
        [Route(ContentApiRoutes.SiteLogo.Get)]
        [AllowAnonymous]
        public async Task<IActionResult> Get(CancellationToken ct) =>
            ToActionResult(await _mediator.Send(new GetSiteLogoQuery(), ct));

        [HttpPut]
        [Route(ContentApiRoutes.SiteLogo.Upsert)]
        [RoleAuthorize(UserType.Admin)]
        public async Task<IActionResult> Upsert([FromBody] UpsertSiteLogoCommand cmd, CancellationToken ct) =>
            ToActionResult(await _mediator.Send(cmd, ct));
    }
}
