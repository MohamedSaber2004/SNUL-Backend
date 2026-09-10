using MediatR;
using SNUL.Shared.Common.DTOs.Commerce;
using SNUL.Shared.Results;

namespace Commerce.Services.API.Features.Carts.Queries.GetCartByUser
{
    public class GetCartByUserQuery : IRequest<Result<CartDto>>
    {
        public Guid UserId { get; set; }
    }
}
