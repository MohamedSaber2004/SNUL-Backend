using MediatR;
using SNUL.Shared.Common.DTOs.Products;
using SNUL.Shared.Common.Repositories.Interfaces.Base;
using SNUL.Shared.Localization;
using SNUL.Shared.Results;
using CategoryEntity = SNUL.Shared.Domain.Models.Category;

namespace Product.Services.API.Features.Categories.Queries.GetCategoryById
{
    public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, Result<CategoryDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetCategoryByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<CategoryDto>> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            var categoryRepo = _unitOfWork.GetRepository<CategoryEntity, Guid>();
            var category = await categoryRepo.GetByIdAsync(request.Id, cancellationToken);

            if (category == null || category.IsDeleted)
            {
                return Result<CategoryDto>.NotFound(LocalizationKeys.Category.NotFound);
            }

            return Result<CategoryDto>.Success(ToDto(category), LocalizationKeys.Category.Fetched);
        }

        internal static CategoryDto ToDto(CategoryEntity category)
        {
            return new CategoryDto
            {
                Id = category.Id,
                NameEn = category.NameEn,
                NameAr = category.NameAr,
                Description = category.Description,
                ImageName = category.ImageName,
                ParentCategoryId = category.ParentCategoryId,
                IsActive = category.IsActive,
                CreatedAt = category.CreatedAt,
                UpdatedAt = category.UpdatedAt
            };
        }
    }
}
