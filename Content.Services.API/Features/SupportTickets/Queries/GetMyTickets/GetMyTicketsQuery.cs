using MediatR;
using SNUL.Shared.Common.DTOs.Content;
using SNUL.Shared.Results;

namespace Content.Services.API.Features.SupportTickets.Queries.GetMyTickets
{
    public class GetMyTicketsQuery : IRequest<Result<List<SupportTicketDto>>>
    {
    }
}
