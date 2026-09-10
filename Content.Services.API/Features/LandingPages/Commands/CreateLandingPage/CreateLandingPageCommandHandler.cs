using Content.Services.API.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SNUL.Shared.Common.DTOs.Content;
using SNUL.Shared.Common.Interfaces;
using SNUL.Shared.Common.Repositories.Interfaces.Base;
using SNUL.Shared.Localization;
using SNUL.Shared.Results;
using LandingPageEntity = SNUL.Shared.Domain.Models.LandingPage;

namespace Content.Services.API.Features.LandingPages.Commands.CreateLandingPage
{
    public class CreateLandingPageCommandHandler : IRequestHandler<CreateLandingPageCommand, Result<LandingPageDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly ICurrentUserService _currentUser;

        public CreateLandingPageCommandHandler(IUnitOfWork uow, ICurrentUserService currentUser)
        {
            _uow = uow;
            _currentUser = currentUser;
        }

        public async Task<Result<LandingPageDto>> Handle(CreateLandingPageCommand request, CancellationToken cancellationToken)
        {
            var repo = _uow.GetRepository<LandingPageEntity, Guid>();
            var slug = request.Slug.Trim().ToLowerInvariant();
            var exists = await repo.ExistsAsync(x => !x.IsDeleted && x.Slug.ToLower() == slug, cancellationToken);
            if (exists)
                return Result<LandingPageDto>.Conflict(LocalizationKeys.LandingPage.SlugAlreadyExists);

            var currentUserId = _currentUser.UserId != Guid.Empty ? _currentUser.UserId.ToString() : "System";

            var entity = new LandingPageEntity
            {
                Id = Guid.NewGuid(),
                Type = request.Type.Trim(),
                Slug = slug,
                HeroTitle = request.HeroTitle.Trim(),
                HeroBody = request.HeroBody,
                ContentBlock = request.ContentBlock
            };
            entity.MarkAsCreated(currentUserId);

            await repo.AddAsync(entity, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            var dto = await repo.GetAll(x => !x.IsDeleted && x.Id == entity.Id)
                .Select(ContentDtoMapper.LandingPageProjection)
                .FirstOrDefaultAsync(cancellationToken);

            return Result<LandingPageDto>.Created(dto!, LocalizationKeys.LandingPage.Created);
        }
    }
}
