using Content.Services.API.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SNUL.Shared.Common.DTOs.Content;
using SNUL.Shared.Common.Interfaces;
using SNUL.Shared.Common.Repositories.Interfaces.Base;
using SNUL.Shared.Localization;
using SNUL.Shared.Results;
using SupportTicketEntity = SNUL.Shared.Domain.Models.SupportTicket;

namespace Content.Services.API.Features.SupportTickets.Commands.CloseTicket
{
    public class CloseTicketCommandHandler : IRequestHandler<CloseTicketCommand, Result<SupportTicketDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly ICurrentUserService _currentUser;
        public CloseTicketCommandHandler(IUnitOfWork uow, ICurrentUserService currentUser) { _uow = uow; _currentUser = currentUser; }

        public async Task<Result<SupportTicketDto>> Handle(CloseTicketCommand request, CancellationToken cancellationToken)
        {
            var repo = _uow.GetRepository<SupportTicketEntity, Guid>();
            var entity = await repo.GetByIdAsync(request.Id, cancellationToken);
            if (entity == null || entity.IsDeleted) return Result<SupportTicketDto>.NotFound(LocalizationKeys.SupportTicket.NotFound);
            var currentUserId = _currentUser.UserId != Guid.Empty ? _currentUser.UserId.ToString() : "System";
            entity.Status = "Closed";
            entity.MarkAsUpdated(currentUserId);
            repo.Update(entity);
            await _uow.SaveChangesAsync(cancellationToken);
            var dto = await repo.GetAll(t => !t.IsDeleted && t.Id == entity.Id)
                .Select(ContentDtoMapper.SupportTicketProjection)
                .FirstOrDefaultAsync(cancellationToken);
            return Result<SupportTicketDto>.Success(dto!, LocalizationKeys.SupportTicket.Closed);
        }
    }
}
