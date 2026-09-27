using MediatR;
using SNUL.Shared.Common.DTOs.Products;
using SNUL.Shared.Results;

namespace Product.Services.API.Features.Categories.Queries.GetAllCategories
{
    /// <summary>Unpaginated category list, for filter dropdowns and tree building.</summary>
    public class GetAllCategoriesQuery : IRequest<Result<List<CategoryDto>>>
    {
    }
}
