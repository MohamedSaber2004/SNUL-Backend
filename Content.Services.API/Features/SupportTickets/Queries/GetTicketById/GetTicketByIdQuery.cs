using MediatR;
using SNUL.Shared.Common.DTOs.Content;
using SNUL.Shared.Results;

namespace Content.Services.API.Features.SupportTickets.Queries.GetTicketById
{
    public class GetTicketByIdQuery : IRequest<Result<SupportTicketDto>>
    {
        public Guid Id { get; set; }
    }
}
