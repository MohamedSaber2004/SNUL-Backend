using MediatR;
using SNUL.Shared.Common.DTOs.Content;
using SNUL.Shared.Results;

namespace Content.Services.API.Features.SupportTickets.Commands.CloseTicket
{
    public class CloseTicketCommand : IRequest<Result<SupportTicketDto>>
    {
        public Guid Id { get; set; }
    }
}
