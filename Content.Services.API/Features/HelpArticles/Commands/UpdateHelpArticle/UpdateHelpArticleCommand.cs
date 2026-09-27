using MediatR;
using SNUL.Shared.Common.DTOs.Content;
using SNUL.Shared.Results;

namespace Content.Services.API.Features.HelpArticles.Commands.UpdateHelpArticle
{
    public class UpdateHelpArticleCommand : IRequest<Result<HelpArticleDto>>
    {
        public Guid Id { get; set; }
        public Guid CategoryId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? TitleAr { get; set; }
        public string Body { get; set; } = string.Empty;
        public string? BodyAr { get; set; }
        public string Slug { get; set; } = string.Empty;
        public bool? IsActive { get; set; }
    }
}
