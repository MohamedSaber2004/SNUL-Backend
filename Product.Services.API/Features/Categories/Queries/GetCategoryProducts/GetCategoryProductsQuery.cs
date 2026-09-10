using MediatR;
using SNUL.Shared.Common.DTOs.Products;
using SNUL.Shared.Results;

namespace Product.Services.API.Features.Categories.Queries.GetCategoryProducts
{
    public class GetCategoryProductsQuery : IRequest<Result<List<ProductDto>>>
    {
        public Guid CategoryId { get; set; }
    }
}
