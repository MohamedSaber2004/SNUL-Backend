using MediatR;
using SNUL.Shared.Common.DTOs.Products;
using SNUL.Shared.Results;

namespace Product.Services.API.Features.Products.Queries.GetProductById
{
    public class GetProductByIdQuery : IRequest<Result<ProductDto>>
    {
        public Guid Id { get; set; }
    }
}
