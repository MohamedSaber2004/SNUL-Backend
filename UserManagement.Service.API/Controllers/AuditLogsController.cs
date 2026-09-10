using MediatR;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Service.API.Features.AuditLogs.Queries.GetAuditLogs.GetAuditLogById;
using UserManagement.Service.API.Features.AuditLogs.Queries.GetAuditLogs.GetAuditLogs;
using UserManagement.Service.API.UserManagementRoutes;
using SNUL.Shared.Common.Attributes;
using SNUL.Shared.Controllers;
using SNUL.Shared.Enums;

namespace UserManagement.Service.API.Controllers
{
    [RoleAuthorize]
    [Route(UserManagementApiRoutes.AuditLogs.Base)]
    public class AuditLogsController : AppControllerBase
    {
        public AuditLogsController(IMediator mediator) : base(mediator) { }

        [HttpGet]
        [Route(UserManagementApiRoutes.AuditLogs.GetAll)]
        [RoleAuthorize(UserType.Admin, UserType.SnulStaff)]
        public async Task<IActionResult> GetAll([FromQuery] GetAuditLogsQuery query, CancellationToken ct)
            => ToActionResult(await _mediator.Send(query, ct));

        [HttpGet]
        [Route(UserManagementApiRoutes.AuditLogs.GetById)]
        [RoleAuthorize(UserType.Admin, UserType.SnulStaff)]
        public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetAuditLogByIdQuery { Id = id }, ct);
            return ToActionResult(result);
        }
    }
}
