using MediatR;
using SNUL.Shared.Common.DTOs.Content;
using SNUL.Shared.Results;

namespace Content.Services.API.Features.HelpArticles.Queries.GetHelpArticles
{
    public class GetHelpArticlesQuery : IRequest<Result<List<HelpArticleDto>>>
    {
        public Guid? CategoryId { get; set; }
    }
}
