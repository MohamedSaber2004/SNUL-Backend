using MediatR;
using SNUL.Shared.Common.DTOs.Commerce;
using SNUL.Shared.Results;

namespace Commerce.Services.API.Features.Carts.Queries.GetCartById
{
    public class GetCartByIdQuery : IRequest<Result<CartDto>>
    {
        public Guid Id { get; set; }
    }
}
