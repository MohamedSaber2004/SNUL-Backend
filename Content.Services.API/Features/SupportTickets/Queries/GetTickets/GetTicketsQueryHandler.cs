using Content.Services.API.Common;
using MediatR;
using SNUL.Shared.Common.DTOs.Content;
using SNUL.Shared.Common.Extensions;
using SNUL.Shared.Common.Repositories.Interfaces.Base;
using SNUL.Shared.Localization;
using SNUL.Shared.Results;
using SupportTicketEntity = SNUL.Shared.Domain.Models.SupportTicket;

namespace Content.Services.API.Features.SupportTickets.Queries.GetTickets
{
    public class GetTicketsQueryHandler : IRequestHandler<GetTicketsQuery, PaginatedResult<SupportTicketDto>>
    {
        private readonly IUnitOfWork _uow;
        public GetTicketsQueryHandler(IUnitOfWork uow) => _uow = uow;

        public async Task<PaginatedResult<SupportTicketDto>> Handle(GetTicketsQuery request, CancellationToken cancellationToken)
        {
            var repo = _uow.GetRepository<SupportTicketEntity, Guid>();
            var q = repo.GetAll(t => !t.IsDeleted);
            if (!string.IsNullOrWhiteSpace(request.Status))
                q = q.Where(t => t.Status == request.Status);
            return await q.OrderByDescending(t => t.CreatedAt)
                .ToPaginatedListAsync(ContentDtoMapper.SupportTicketProjection, request.PageNumber, request.PageSize, LocalizationKeys.SupportTicket.ListFetched, cancellationToken);
        }
    }
}
