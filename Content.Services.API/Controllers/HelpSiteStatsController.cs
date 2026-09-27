using Content.Services.API.ContentRoutes;
using Content.Services.API.Features.HelpSiteStats.Commands.CreateHelpSiteStat;
using Content.Services.API.Features.HelpSiteStats.Commands.DeleteHelpSiteStat;
using Content.Services.API.Features.HelpSiteStats.Commands.UpdateHelpSiteStat;
using Content.Services.API.Features.HelpSiteStats.Queries.GetHelpSiteStatByKey;
using Content.Services.API.Features.HelpSiteStats.Queries.GetHelpSiteStats;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SNUL.Shared.Common.Attributes;
using SNUL.Shared.Controllers;
using SNUL.Shared.Enums;

namespace Content.Services.API.Controllers
{
    [RoleAuthorize]
    [Route(ContentApiRoutes.HelpSiteStats.Base)]
    public class HelpSiteStatsController : AppControllerBase
    {
        public HelpSiteStatsController(IMediator mediator) : base(mediator) { }

        [HttpGet]
        [Route(ContentApiRoutes.HelpSiteStats.GetAll)]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll(CancellationToken ct) => ToActionResult(await _mediator.Send(new GetHelpSiteStatsQuery(), ct));

        [HttpGet]
        [Route(ContentApiRoutes.HelpSiteStats.GetByKey)]
        [AllowAnonymous]
        public async Task<IActionResult> GetByKey([FromRoute] string key, CancellationToken ct) => ToActionResult(await _mediator.Send(new GetHelpSiteStatByKeyQuery { Key = key }, ct));

        [HttpPost]
        [Route(ContentApiRoutes.HelpSiteStats.Create)]
        [RoleAuthorize(UserType.Admin)]
        public async Task<IActionResult> Create([FromBody] CreateHelpSiteStatCommand cmd, CancellationToken ct) => ToActionResult(await _mediator.Send(cmd, ct));

        [HttpPut]
        [Route(ContentApiRoutes.HelpSiteStats.Update)]
        [RoleAuthorize(UserType.Admin)]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateHelpSiteStatCommand cmd, CancellationToken ct)
        {
            cmd.Id = id;
            return ToActionResult(await _mediator.Send(cmd, ct));
        }

        [HttpDelete]
        [Route(ContentApiRoutes.HelpSiteStats.Delete)]
        [RoleAuthorize(UserType.Admin)]
        public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken ct) => ToActionResult(await _mediator.Send(new DeleteHelpSiteStatCommand { Id = id }, ct));
    }
}
