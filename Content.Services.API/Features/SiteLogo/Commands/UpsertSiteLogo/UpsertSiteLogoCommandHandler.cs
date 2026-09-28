using MediatR;
using Microsoft.EntityFrameworkCore;
using SNUL.Shared.Common.DTOs.Content;
using SNUL.Shared.Common.Repositories.Interfaces.Base;
using SNUL.Shared.Localization;
using SNUL.Shared.Results;
using SiteLogoEntity = SNUL.Shared.Domain.Models.SiteLogo;

namespace Content.Services.API.Features.SiteLogo.Commands.UpsertSiteLogo
{
    public class UpsertSiteLogoCommandHandler : IRequestHandler<UpsertSiteLogoCommand, Result<SiteLogoDto>>
    {
        private readonly IUnitOfWork _uow;
        public UpsertSiteLogoCommandHandler(IUnitOfWork uow) => _uow = uow;

        public async Task<Result<SiteLogoDto>> Handle(UpsertSiteLogoCommand request, CancellationToken cancellationToken)
        {
            var repo = _uow.GetRepository<SiteLogoEntity, Guid>();

            var existing = await repo.GetAll(s => !s.IsDeleted)
                .OrderByDescending(s => s.UpdatedAt ?? s.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);

            SiteLogoEntity logo;
            if (existing != null)
            {
                existing.LogoUrl = (request.LogoUrl ?? string.Empty).Trim();
                existing.AltText = string.IsNullOrWhiteSpace(request.AltText) ? "SNUL" : request.AltText.Trim();
                repo.Update(existing);
                logo = existing;
            }
            else
            {
                logo = new SiteLogoEntity
                {
                    Id = Guid.NewGuid(),
                    LogoUrl = (request.LogoUrl ?? string.Empty).Trim(),
                    AltText = string.IsNullOrWhiteSpace(request.AltText) ? "SNUL" : request.AltText.Trim()
                };
                await repo.AddAsync(logo, cancellationToken);
            }

            await _uow.SaveChangesAsync(cancellationToken);

            return Result<SiteLogoDto>.Success(new SiteLogoDto
            {
                Id = logo.Id,
                LogoUrl = logo.LogoUrl,
                AltText = logo.AltText,
                UpdatedAt = logo.UpdatedAt ?? logo.CreatedAt
            }, LocalizationKeys.SiteLogo.Updated);
        }
    }
}
