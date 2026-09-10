using MediatR;
using SNUL.Shared.Common.DTOs.Content;
using SNUL.Shared.Results;

namespace Content.Services.API.Features.HelpCategories.Commands.CreateHelpCategory
{
    public class CreateHelpCategoryCommand : IRequest<Result<HelpCategoryDto>>
    {
        public string Name { get; set; } = string.Empty;
        public string? Icon { get; set; }
    }
}
