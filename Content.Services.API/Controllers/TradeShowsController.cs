using Content.Services.API.Features.TradeShows.Commands.CreateTradeShow;
using Content.Services.API.Features.TradeShows.Queries.GetTradeShows;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SNUL.Shared.Common.Attributes;
using SNUL.Shared.Controllers;
using SNUL.Shared.Enums;

namespace Content.Services.API.Controllers
{
    [Route("api/v1/trade-shows")]
    public class TradeShowsController : AppControllerBase
    {
        public TradeShowsController(IMediator mediator) : base(mediator) { }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll([FromQuery] bool? upcomingOnly, CancellationToken ct)
            => ToActionResult(await _mediator.Send(new GetTradeShowsQuery { UpcomingOnly = upcomingOnly }, ct));

        [HttpPost]
        [RoleAuthorize(UserType.Admin, UserType.SnulStaff)]
        public async Task<IActionResult> Create([FromBody] CreateTradeShowCommand cmd, CancellationToken ct)
            => ToActionResult(await _mediator.Send(cmd, ct));
    }
}
