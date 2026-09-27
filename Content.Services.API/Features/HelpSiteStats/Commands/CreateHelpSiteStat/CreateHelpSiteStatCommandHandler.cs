using Content.Services.API.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SNUL.Shared.Common.DTOs.Content;
using SNUL.Shared.Common.Interfaces;
using SNUL.Shared.Common.Repositories.Interfaces.Base;
using SNUL.Shared.Localization;
using SNUL.Shared.Results;
using HelpSiteStatEntity = SNUL.Shared.Domain.Models.HelpSiteStat;

namespace Content.Services.API.Features.HelpSiteStats.Commands.CreateHelpSiteStat
{
    public class CreateHelpSiteStatCommandHandler : IRequestHandler<CreateHelpSiteStatCommand, Result<HelpSiteStatDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly ICurrentUserService _currentUser;
        public CreateHelpSiteStatCommandHandler(IUnitOfWork uow, ICurrentUserService currentUser) { _uow = uow; _currentUser = currentUser; }

        public async Task<Result<HelpSiteStatDto>> Handle(CreateHelpSiteStatCommand request, CancellationToken cancellationToken)
        {
            var repo = _uow.GetRepository<HelpSiteStatEntity, Guid>();
            var statKey = request.StatKey.Trim().ToLowerInvariant();

            // StatKey is unique and is the frontend contract, so two rows claiming the
            // same slot would make the hero ambiguous. Check it here; the unique index
            // on HelpSiteStats.StatKey is the backstop against a race.
            var exists = await repo.ExistsAsync(s => !s.IsDeleted && s.StatKey.ToLower() == statKey, cancellationToken);
            if (exists)
                return Result<HelpSiteStatDto>.Conflict(LocalizationKeys.HelpSiteStat.StatKeyAlreadyExists);

            var currentUserId = _currentUser.UserId != Guid.Empty ? _currentUser.UserId.ToString() : "System";

            var entity = new HelpSiteStatEntity
            {
                Id = Guid.NewGuid(),
                StatKey = statKey,
                Value = request.Value.Trim(),
                Label = request.Label.Trim(),
                LabelAr = request.LabelAr?.Trim(),
                IsVisible = request.IsVisible,
                SortOrder = request.SortOrder
            };
            entity.MarkAsCreated(currentUserId);

            await repo.AddAsync(entity, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            var dto = await repo.GetAll(s => !s.IsDeleted && s.Id == entity.Id)
                .Select(ContentDtoMapper.HelpSiteStatProjection)
                .FirstOrDefaultAsync(cancellationToken);

            return Result<HelpSiteStatDto>.Created(dto!, LocalizationKeys.HelpSiteStat.Created);
        }
    }
}
