using Content.Services.API.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SNUL.Shared.Common.DTOs.Content;
using SNUL.Shared.Common.Repositories.Interfaces.Base;
using SNUL.Shared.Localization;
using SNUL.Shared.Results;
using LandingPageEntity = SNUL.Shared.Domain.Models.LandingPage;

namespace Content.Services.API.Features.LandingPages.Queries.GetLandingPageBySlug
{
    public class GetLandingPageBySlugQueryHandler : IRequestHandler<GetLandingPageBySlugQuery, Result<LandingPageDto>>
    {
        private readonly IUnitOfWork _uow;
        public GetLandingPageBySlugQueryHandler(IUnitOfWork uow) => _uow = uow;

        public async Task<Result<LandingPageDto>> Handle(GetLandingPageBySlugQuery request, CancellationToken cancellationToken)
        {
            var repo = _uow.GetRepository<LandingPageEntity, Guid>();
            var slug = request.Slug.Trim().ToLowerInvariant();
            var dto = await repo.GetAll(x => !x.IsDeleted && x.IsActive && x.Slug.ToLower() == slug)
                .Select(ContentDtoMapper.LandingPageProjection)
                .FirstOrDefaultAsync(cancellationToken);

            if (dto == null)
                return Result<LandingPageDto>.NotFound(LocalizationKeys.LandingPage.NotFound);

            return Result<LandingPageDto>.Success(dto, LocalizationKeys.LandingPage.Fetched);
        }
    }
}
