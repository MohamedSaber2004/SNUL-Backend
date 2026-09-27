using Content.Services.API.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SNUL.Shared.Common.DTOs.Content;
using SNUL.Shared.Common.Interfaces;
using SNUL.Shared.Common.Repositories.Interfaces.Base;
using SNUL.Shared.Localization;
using SNUL.Shared.Results;
using HelpSiteStatEntity = SNUL.Shared.Domain.Models.HelpSiteStat;

namespace Content.Services.API.Features.HelpSiteStats.Commands.UpdateHelpSiteStat
{
    public class UpdateHelpSiteStatCommandHandler : IRequestHandler<UpdateHelpSiteStatCommand, Result<HelpSiteStatDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly ICurrentUserService _currentUser;
        public UpdateHelpSiteStatCommandHandler(IUnitOfWork uow, ICurrentUserService currentUser) { _uow = uow; _currentUser = currentUser; }

        public async Task<Result<HelpSiteStatDto>> Handle(UpdateHelpSiteStatCommand request, CancellationToken cancellationToken)
        {
            var repo = _uow.GetRepository<HelpSiteStatEntity, Guid>();
            var entity = await repo.GetByIdAsync(request.Id, cancellationToken);
            if (entity == null || entity.IsDeleted) return Result<HelpSiteStatDto>.NotFound(LocalizationKeys.HelpSiteStat.NotFound);

            var currentUserId = _currentUser.UserId != Guid.Empty ? _currentUser.UserId.ToString() : "System";
            entity.Value = request.Value.Trim();
            entity.Label = request.Label.Trim();
            entity.LabelAr = request.LabelAr?.Trim();
            entity.IsVisible = request.IsVisible;
            entity.SortOrder = request.SortOrder;
            entity.MarkAsUpdated(currentUserId);

            repo.Update(entity);
            await _uow.SaveChangesAsync(cancellationToken);

            var dto = await repo.GetAll(s => !s.IsDeleted && s.Id == entity.Id)
                .Select(ContentDtoMapper.HelpSiteStatProjection)
                .FirstOrDefaultAsync(cancellationToken);

            return Result<HelpSiteStatDto>.Success(dto!, LocalizationKeys.HelpSiteStat.Updated);
        }
    }
}
