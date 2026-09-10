using MediatR;
using SNUL.Shared.Common.DTOs.UserManagement;
using SNUL.Shared.Results;

namespace UserManagement.Service.API.Features.AuditLogs.Queries.GetAuditLogs.GetAuditLogs
{
    public class GetAuditLogsQuery : IRequest<PaginatedResult<AuditLogDto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? EntityName { get; set; }
        public string? Action { get; set; }
        public string? PerformedBy { get; set; }
        public string? SearchTerm { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
