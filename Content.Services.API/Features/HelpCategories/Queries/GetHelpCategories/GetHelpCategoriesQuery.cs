using MediatR;
using SNUL.Shared.Common.DTOs.Content;
using SNUL.Shared.Results;

namespace Content.Services.API.Features.HelpCategories.Queries.GetHelpCategories
{
    public class GetHelpCategoriesQuery : IRequest<Result<List<HelpCategoryDto>>>
    {
    }
}
