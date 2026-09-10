using Content.Services.API.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SNUL.Shared.Common.DTOs.Content;
using SNUL.Shared.Common.Repositories.Interfaces.Base;
using SNUL.Shared.Localization;
using SNUL.Shared.Results;
using HelpArticleEntity = SNUL.Shared.Domain.Models.HelpArticle;

namespace Content.Services.API.Features.HelpArticles.Queries.GetHelpArticleById
{
    public class GetHelpArticleByIdQueryHandler : IRequestHandler<GetHelpArticleByIdQuery, Result<HelpArticleDto>>
    {
        private readonly IUnitOfWork _uow;
        public GetHelpArticleByIdQueryHandler(IUnitOfWork uow) => _uow = uow;

        public async Task<Result<HelpArticleDto>> Handle(GetHelpArticleByIdQuery request, CancellationToken cancellationToken)
        {
            var repo = _uow.GetRepository<HelpArticleEntity, Guid>();
            var dto = await repo.GetAll(a => !a.IsDeleted && a.Id == request.Id)
                .Select(ContentDtoMapper.HelpArticleProjection)
                .FirstOrDefaultAsync(cancellationToken);
            if (dto == null) return Result<HelpArticleDto>.NotFound(LocalizationKeys.HelpArticle.NotFound);
            return Result<HelpArticleDto>.Success(dto, LocalizationKeys.HelpArticle.Fetched);
        }
    }
}
