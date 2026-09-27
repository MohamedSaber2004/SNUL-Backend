using MediatR;
using SNUL.Shared.Common.DTOs.Products;
using SNUL.Shared.Results;

namespace Product.Services.API.Features.Products.Queries.GetMostSellingProducts
{
    public class GetMostSellingProductsQuery : IRequest<Result<IReadOnlyList<ProductDto>>>
    {
        public int Limit { get; set; } = 7;
        public int DaysWindow { get; set; } = 7;
    }
}
