using Content.Services.API.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SNUL.Shared.Common.DTOs.Content;
using SNUL.Shared.Common.Repositories.Interfaces.Base;
using SNUL.Shared.Localization;
using SNUL.Shared.Results;
using HelpArticleEntity = SNUL.Shared.Domain.Models.HelpArticle;

namespace Content.Services.API.Features.HelpArticles.Queries.GetHelpArticleBySlug
{
    public class GetHelpArticleBySlugQueryHandler : IRequestHandler<GetHelpArticleBySlugQuery, Result<HelpArticleDto>>
    {
        private readonly IUnitOfWork _uow;
        public GetHelpArticleBySlugQueryHandler(IUnitOfWork uow) => _uow = uow;

        public async Task<Result<HelpArticleDto>> Handle(GetHelpArticleBySlugQuery request, CancellationToken cancellationToken)
        {
            var slug = request.Slug.Trim().ToLowerInvariant();
            var repo = _uow.GetRepository<HelpArticleEntity, Guid>();
            var dto = await repo.GetAll(a => !a.IsDeleted && a.Slug.ToLower() == slug)
                .Select(ContentDtoMapper.HelpArticleProjection)
                .FirstOrDefaultAsync(cancellationToken);
            if (dto == null) return Result<HelpArticleDto>.NotFound(LocalizationKeys.HelpArticle.NotFound);
            return Result<HelpArticleDto>.Success(dto, LocalizationKeys.HelpArticle.Fetched);
        }
    }
}
