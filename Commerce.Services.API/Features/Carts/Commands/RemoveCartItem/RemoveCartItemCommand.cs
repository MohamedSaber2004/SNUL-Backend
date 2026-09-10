using MediatR;
using SNUL.Shared.Common.DTOs.Commerce;
using SNUL.Shared.Results;

namespace Commerce.Services.API.Features.Carts.Commands.RemoveCartItem
{
    public class RemoveCartItemCommand : IRequest<Result<CartDto>>
    {
        public Guid CartId { get; set; }
        public Guid ItemId { get; set; }
    }
}
