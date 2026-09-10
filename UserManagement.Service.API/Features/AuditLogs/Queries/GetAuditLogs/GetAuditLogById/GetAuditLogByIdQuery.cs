using MediatR;
using SNUL.Shared.Common.DTOs.UserManagement;
using SNUL.Shared.Results;

namespace UserManagement.Service.API.Features.AuditLogs.Queries.GetAuditLogs.GetAuditLogById
{
    public class GetAuditLogByIdQuery : IRequest<Result<AuditLogDto>>
    {
        public Guid Id { get; set; }
    }
}
