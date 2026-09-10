using Content.Services.API.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SNUL.Shared.Common.DTOs.Content;
using SNUL.Shared.Common.Interfaces;
using SNUL.Shared.Common.Repositories.Interfaces.Base;
using SNUL.Shared.Localization;
using SNUL.Shared.Results;
using SupportTicketEntity = SNUL.Shared.Domain.Models.SupportTicket;

namespace Content.Services.API.Features.SupportTickets.Commands.ReplyTicket
{
    public class ReplyTicketCommandHandler : IRequestHandler<ReplyTicketCommand, Result<SupportTicketDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly ICurrentUserService _currentUser;
        public ReplyTicketCommandHandler(IUnitOfWork uow, ICurrentUserService currentUser) { _uow = uow; _currentUser = currentUser; }

        public async Task<Result<SupportTicketDto>> Handle(ReplyTicketCommand request, CancellationToken cancellationToken)
        {
            var repo = _uow.GetRepository<SupportTicketEntity, Guid>();
            var entity = await repo.GetByIdAsync(request.Id, cancellationToken);
            if (entity == null || entity.IsDeleted) return Result<SupportTicketDto>.NotFound(LocalizationKeys.SupportTicket.NotFound);
            var currentUserId = _currentUser.UserId != Guid.Empty ? _currentUser.UserId.ToString() : "System";
            entity.Reply = request.Reply.Trim();
            entity.RepliedAt = DateTime.UtcNow;
            entity.RepliedBy = _currentUser.UserId;
            entity.Status = "Answered";
            entity.MarkAsUpdated(currentUserId);
            repo.Update(entity);
            await _uow.SaveChangesAsync(cancellationToken);
            var dto = await repo.GetAll(t => !t.IsDeleted && t.Id == entity.Id)
                .Select(ContentDtoMapper.SupportTicketProjection)
                .FirstOrDefaultAsync(cancellationToken);
            return Result<SupportTicketDto>.Success(dto!, LocalizationKeys.SupportTicket.Updated);
        }
    }
}
