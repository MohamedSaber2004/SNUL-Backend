using Content.Services.API.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SNUL.Shared.Common.DTOs.Content;
using SNUL.Shared.Common.Repositories.Interfaces.Base;
using SNUL.Shared.Localization;
using SNUL.Shared.Results;
using SiteLogoEntity = SNUL.Shared.Domain.Models.SiteLogo;

namespace Content.Services.API.Features.SiteLogo.Queries.GetSiteLogo
{
    public class GetSiteLogoQueryHandler : IRequestHandler<GetSiteLogoQuery, Result<SiteLogoDto>>
    {
        private readonly IUnitOfWork _uow;
        public GetSiteLogoQueryHandler(IUnitOfWork uow) => _uow = uow;

        public async Task<Result<SiteLogoDto>> Handle(GetSiteLogoQuery request, CancellationToken cancellationToken)
        {
            var repo = _uow.GetRepository<SiteLogoEntity, Guid>();

            var logo = await repo.GetAll(s => !s.IsDeleted)
                .AsNoTracking()
                .OrderByDescending(s => s.UpdatedAt ?? s.CreatedAt)
                .Select(ContentDtoMapper.SiteLogoProjection)
                .FirstOrDefaultAsync(cancellationToken);

            // No row yet: return a default so the frontend uses the static fallback.
            if (logo == null)
            {
                return Result<SiteLogoDto>.Success(new SiteLogoDto
                {
                    Id = Guid.Empty,
                    LogoUrl = string.Empty,
                    AltText = "SNUL",
                    UpdatedAt = DateTime.UtcNow
                }, LocalizationKeys.SiteLogo.Fetched);
            }

            return Result<SiteLogoDto>.Success(logo, LocalizationKeys.SiteLogo.Fetched);
        }
    }
}
