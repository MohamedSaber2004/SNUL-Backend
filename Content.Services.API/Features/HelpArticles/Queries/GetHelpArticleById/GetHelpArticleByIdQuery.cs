using MediatR;
using SNUL.Shared.Common.DTOs.Content;
using SNUL.Shared.Results;

namespace Content.Services.API.Features.HelpArticles.Queries.GetHelpArticleById
{
    public class GetHelpArticleByIdQuery : IRequest<Result<HelpArticleDto>>
    {
        public Guid Id { get; set; }
    }
}
