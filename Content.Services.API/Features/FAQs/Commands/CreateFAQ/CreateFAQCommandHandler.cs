using Content.Services.API.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SNUL.Shared.Common.DTOs.Content;
using SNUL.Shared.Common.Interfaces;
using SNUL.Shared.Common.Repositories.Interfaces.Base;
using SNUL.Shared.Localization;
using SNUL.Shared.Results;
using FAQEntity = SNUL.Shared.Domain.Models.FAQItem;

namespace Content.Services.API.Features.FAQs.Commands.CreateFAQ
{
    public class CreateFAQCommandHandler : IRequestHandler<CreateFAQCommand, Result<FAQItemDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly ICurrentUserService _currentUser;
        public CreateFAQCommandHandler(IUnitOfWork uow, ICurrentUserService currentUser) { _uow = uow; _currentUser = currentUser; }

        public async Task<Result<FAQItemDto>> Handle(CreateFAQCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUser.UserId != Guid.Empty ? _currentUser.UserId.ToString() : "System";
            var entity = new FAQEntity
            {
                Id = Guid.NewGuid(),
                Question = request.Question.Trim(),
                Answer = request.Answer.Trim(),
                SortOrder = request.SortOrder
            };
            entity.MarkAsCreated(currentUserId);
            var repo = _uow.GetRepository<FAQEntity, Guid>();
            await repo.AddAsync(entity, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);
            var dto = await repo.GetAll(f => !f.IsDeleted && f.Id == entity.Id)
                .Select(ContentDtoMapper.FAQProjection)
                .FirstOrDefaultAsync(cancellationToken);
            return Result<FAQItemDto>.Created(dto!, LocalizationKeys.FAQ.Created);
        }
    }
}
