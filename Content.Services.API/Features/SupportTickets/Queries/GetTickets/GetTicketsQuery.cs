using MediatR;
using SNUL.Shared.Common.DTOs.Content;
using SNUL.Shared.Results;

namespace Content.Services.API.Features.SupportTickets.Queries.GetTickets
{
    public class GetTicketsQuery : IRequest<PaginatedResult<SupportTicketDto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? Status { get; set; }
    }
}
