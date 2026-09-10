using MediatR;
using Microsoft.EntityFrameworkCore;
using SNUL.Shared.Common.DTOs.UserManagement;
using SNUL.Shared.Domain.Models;
using SNUL.Shared.Localization;
using SNUL.Shared.Results;

namespace UserManagement.Service.API.Features.AuditLogs.Queries.GetAuditLogs.GetAuditLogById
{
    public class GetAuditLogByIdQueryHandler : IRequestHandler<GetAuditLogByIdQuery, Result<AuditLogDto>>
    {
        private readonly SNUL.Shared.Persistance.SnulDbContext _context;

        public GetAuditLogByIdQueryHandler(SNUL.Shared.Persistance.SnulDbContext context)
        {
            _context = context;
        }

        public async Task<Result<AuditLogDto>> Handle(GetAuditLogByIdQuery request, CancellationToken cancellationToken)
        {
            var log = await _context.AuditLogs
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == request.Id && a.Id != Guid.Empty, cancellationToken);

            if (log == null)
                return Result<AuditLogDto>.Failure(LocalizationKeys.AuditLog.NotFound);

            return Result<AuditLogDto>.Success(new AuditLogDto
            {
                Id = log.Id,
                EntityName = log.EntityName,
                EntityId = log.EntityId,
                Action = log.Action,
                Details = log.Details,
                PerformedBy = log.PerformedBy ?? "System",
                CreatedAt = log.CreatedAt,
            });
        }
    }
}
