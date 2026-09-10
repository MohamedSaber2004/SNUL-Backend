using MediatR;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Service.API.Features.DistributorApplications.Commands.ApproveDistributorApplication;
using UserManagement.Service.API.Features.DistributorApplications.Commands.CreateDistributorApplication;
using UserManagement.Service.API.Features.DistributorApplications.Commands.RejectDistributorApplication;
using UserManagement.Service.API.Features.DistributorApplications.Queries.GetDistributorApplicationById;
using UserManagement.Service.API.Features.DistributorApplications.Queries.GetDistributorApplications;
using UserManagement.Service.API.UserManagementRoutes;
using SNUL.Shared.Common.Attributes;
using SNUL.Shared.Controllers;
using SNUL.Shared.Enums;

namespace UserManagement.Service.API.Controllers
{
    [Route(UserManagementApiRoutes.DistributorApplications.Base)]
    public class DistributorApplicationsController : AppControllerBase
    {
        public DistributorApplicationsController(IMediator mediator) : base(mediator) { }

[HttpPost]
        [Route(UserManagementApiRoutes.DistributorApplications.Create)]
        [RoleAuthorize]
        public async Task<IActionResult> Create([FromBody] CreateDistributorApplicationCommand cmd, CancellationToken ct)
            => ToActionResult(await _mediator.Send(cmd, ct));

[HttpGet]
        [Route(UserManagementApiRoutes.DistributorApplications.GetAll)]
        [RoleAuthorize(UserType.Admin, UserType.SnulStaff)]
        public async Task<IActionResult> GetAll([FromQuery] GetDistributorApplicationsQuery query, CancellationToken ct)
            => ToActionResult(await _mediator.Send(query, ct));

[HttpGet]
        [Route(UserManagementApiRoutes.DistributorApplications.GetById)]
        [RoleAuthorize(UserType.Admin, UserType.SnulStaff)]
        public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken ct)
            => ToActionResult(await _mediator.Send(new GetDistributorApplicationByIdQuery(id), ct));

[HttpPut]
        [Route(UserManagementApiRoutes.DistributorApplications.Approve)]
        [RoleAuthorize(UserType.Admin)]
        public async Task<IActionResult> Approve([FromRoute] Guid id, [FromBody] ApproveDistributorApplicationCommand? cmd, CancellationToken ct)
        {
            cmd ??= new ApproveDistributorApplicationCommand();
            cmd.Id = id;
            return ToActionResult(await _mediator.Send(cmd, ct));
        }

[HttpPut]
        [Route(UserManagementApiRoutes.DistributorApplications.Reject)]
        [RoleAuthorize(UserType.Admin)]
        public async Task<IActionResult> Reject([FromRoute] Guid id, [FromBody] RejectDistributorApplicationCommand cmd, CancellationToken ct)
        {
            cmd.Id = id;
            return ToActionResult(await _mediator.Send(cmd, ct));
        }
    }
}

