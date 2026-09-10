using Content.Services.API.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SNUL.Shared.Common.DTOs.Content;
using SNUL.Shared.Common.Interfaces;
using SNUL.Shared.Common.Repositories.Interfaces.Base;
using SNUL.Shared.Localization;
using SNUL.Shared.Results;
using SupportTicketEntity = SNUL.Shared.Domain.Models.SupportTicket;

namespace Content.Services.API.Features.SupportTickets.Queries.GetTicketById
{
    public class GetTicketByIdQueryHandler : IRequestHandler<GetTicketByIdQuery, Result<SupportTicketDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly ICurrentUserService _currentUser;
        public GetTicketByIdQueryHandler(IUnitOfWork uow, ICurrentUserService currentUser) { _uow = uow; _currentUser = currentUser; }

        public async Task<Result<SupportTicketDto>> Handle(GetTicketByIdQuery request, CancellationToken cancellationToken)
        {
            var repo = _uow.GetRepository<SupportTicketEntity, Guid>();
            var dto = await repo.GetAll(t => !t.IsDeleted && t.Id == request.Id)
                .Select(ContentDtoMapper.SupportTicketProjection)
                .FirstOrDefaultAsync(cancellationToken);
            if (dto == null) return Result<SupportTicketDto>.NotFound(LocalizationKeys.SupportTicket.NotFound);
            return Result<SupportTicketDto>.Success(dto, LocalizationKeys.SupportTicket.Fetched);
        }
    }
}
