using MediatR;
using SNUL.Shared.Common.DTOs.Products;
using SNUL.Shared.Results;

namespace Product.Services.API.Features.Categories.Queries.GetCategoryById
{
    public class GetCategoryByIdQuery : IRequest<Result<CategoryDto>>
    {
        public Guid Id { get; set; }
    }
}
