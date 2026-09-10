using MediatR;
using Microsoft.EntityFrameworkCore;
using SNUL.Shared.Common.DTOs.Products;
using SNUL.Shared.Common.Extensions;
using SNUL.Shared.Common.Repositories.Interfaces.Base;
using SNUL.Shared.Localization;
using SNUL.Shared.Results;
using CategoryEntity = SNUL.Shared.Domain.Models.Category;

namespace Product.Services.API.Features.Categories.Queries.GetCategories
{
    public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, PaginatedResult<CategoryDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetCategoriesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PaginatedResult<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
        {
            var categoryRepo = _unitOfWork.GetRepository<CategoryEntity, Guid>();
            var query = categoryRepo.GetAll(c => !c.IsDeleted).AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var term = request.SearchTerm.Trim().ToLower();
                query = query.Where(c =>
                    c.NameEn.ToLower().Contains(term) ||
                    c.NameAr.ToLower().Contains(term));
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(c => c.IsActive == request.IsActive.Value);
            }

            return await query
                .OrderBy(c => c.NameEn)
                .ToPaginatedListAsync(
                    c => new CategoryDto
                    {
                        Id = c.Id,
                        NameEn = c.NameEn,
                        NameAr = c.NameAr,
                        Description = c.Description,
                        ImageName = c.ImageName,
                        ParentCategoryId = c.ParentCategoryId,
                        IsActive = c.IsActive,
                        ProductCount = c.Products.Count(p => !p.IsDeleted),
                        CreatedAt = c.CreatedAt,
                        UpdatedAt = c.UpdatedAt
                    },
                    request.PageNumber,
                    request.PageSize,
                    LocalizationKeys.Category.ListFetched,
                    cancellationToken);
        }
    }
}
