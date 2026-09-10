using MediatR;
using SNUL.Shared.Common.DTOs.Content;
using SNUL.Shared.Results;

namespace Content.Services.API.Features.HelpCategories.Queries.GetHelpCategoryById
{
    public class GetHelpCategoryByIdQuery : IRequest<Result<HelpCategoryDto>>
    {
        public Guid Id { get; set; }
    }
}
