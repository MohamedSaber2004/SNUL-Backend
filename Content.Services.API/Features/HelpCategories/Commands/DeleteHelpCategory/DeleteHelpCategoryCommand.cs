using MediatR;
using SNUL.Shared.Results;

namespace Content.Services.API.Features.HelpCategories.Commands.DeleteHelpCategory
{
    public class DeleteHelpCategoryCommand : IRequest<Result<string>>
    {
        public Guid Id { get; set; }
    }
}
