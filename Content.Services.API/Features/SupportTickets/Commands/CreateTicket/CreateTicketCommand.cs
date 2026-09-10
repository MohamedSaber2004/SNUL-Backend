using MediatR;
using SNUL.Shared.Common.DTOs.Content;
using SNUL.Shared.Results;

namespace Content.Services.API.Features.SupportTickets.Commands.CreateTicket
{
    public class CreateTicketCommand : IRequest<Result<SupportTicketDto>>
    {
        public string Subject { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
