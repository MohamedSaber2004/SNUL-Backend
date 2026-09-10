using MediatR;
using SNUL.Shared.Common.DTOs.Commerce;
using SNUL.Shared.Results;
namespace Commerce.Services.API.Features.Carts.Queries.GetCarts
{
    public class GetCartsQuery : IRequest<PaginatedResult<CartDto>> { public int PageNumber { get; set; } = 1; public int PageSize { get; set; } = 10; }
}
