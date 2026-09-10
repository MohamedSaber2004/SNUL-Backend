using MediatR;
using SNUL.Shared.Common.DTOs.Commerce;
using SNUL.Shared.Results;

namespace Commerce.Services.API.Features.Orders.Queries.TrackOrder
{
    public class TrackOrderQuery : IRequest<Result<OrderDto>>
    {
        public string OrderNumber { get; set; } = string.Empty;
    }
}
