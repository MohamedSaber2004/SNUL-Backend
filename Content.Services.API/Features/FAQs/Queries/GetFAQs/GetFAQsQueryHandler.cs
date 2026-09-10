using Content.Services.API.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SNUL.Shared.Common.DTOs.Content;
using SNUL.Shared.Common.Repositories.Interfaces.Base;
using SNUL.Shared.Localization;
using SNUL.Shared.Results;
using FAQEntity = SNUL.Shared.Domain.Models.FAQItem;

namespace Content.Services.API.Features.FAQs.Queries.GetFAQs
{
    public class GetFAQsQueryHandler : IRequestHandler<GetFAQsQuery, Result<List<FAQItemDto>>>
    {
        private readonly IUnitOfWork _uow;
        public GetFAQsQueryHandler(IUnitOfWork uow) => _uow = uow;

        public async Task<Result<List<FAQItemDto>>> Handle(GetFAQsQuery request, CancellationToken cancellationToken)
        {
            var repo = _uow.GetRepository<FAQEntity, Guid>();
            var list = await repo.GetAll(f => !f.IsDeleted)
                .AsNoTracking()
                .OrderBy(f => f.SortOrder)
                .ThenBy(f => f.CreatedAt)
                .Select(ContentDtoMapper.FAQProjection)
                .ToListAsync(cancellationToken);
            return Result<List<FAQItemDto>>.Success(list, LocalizationKeys.FAQ.ListFetched);
        }
    }
}
