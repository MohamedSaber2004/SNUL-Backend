using MediatR;
using SNUL.Shared.Common.DTOs.Products;
using SNUL.Shared.Results;

namespace Product.Services.API.Features.Wishlist.Queries.GetWishlist
{
    public class GetWishlistQuery : IRequest<Result<IReadOnlyList<ProductDto>>>
    {
    }
}
