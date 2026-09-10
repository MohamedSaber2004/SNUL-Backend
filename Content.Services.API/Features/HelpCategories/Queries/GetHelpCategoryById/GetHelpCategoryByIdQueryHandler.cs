using Content.Services.API.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SNUL.Shared.Common.DTOs.Content;
using SNUL.Shared.Common.Repositories.Interfaces.Base;
using SNUL.Shared.Localization;
using SNUL.Shared.Results;
using HelpCategoryEntity = SNUL.Shared.Domain.Models.HelpCategory;

namespace Content.Services.API.Features.HelpCategories.Queries.GetHelpCategoryById
{
    public class GetHelpCategoryByIdQueryHandler : IRequestHandler<GetHelpCategoryByIdQuery, Result<HelpCategoryDto>>
    {
        private readonly IUnitOfWork _uow;
        public GetHelpCategoryByIdQueryHandler(IUnitOfWork uow) => _uow = uow;

        public async Task<Result<HelpCategoryDto>> Handle(GetHelpCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            var repo = _uow.GetRepository<HelpCategoryEntity, Guid>();
            var dto = await repo.GetAll(c => !c.IsDeleted && c.Id == request.Id)
                .Select(ContentDtoMapper.HelpCategoryProjection)
                .FirstOrDefaultAsync(cancellationToken);
            if (dto == null) return Result<HelpCategoryDto>.NotFound(LocalizationKeys.HelpCategory.NotFound);
            return Result<HelpCategoryDto>.Success(dto, LocalizationKeys.HelpCategory.Fetched);
        }
    }
}
