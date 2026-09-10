using MediatR;
using SNUL.Shared.Common.DTOs.Products;
using SNUL.Shared.Common.Interfaces;
using SNUL.Shared.Common.Repositories.Interfaces.Base;
using SNUL.Shared.Localization;
using SNUL.Shared.Results;
using CategoryEntity = SNUL.Shared.Domain.Models.Category;

namespace Product.Services.API.Features.Categories.Commands.CreateCategory
{
    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Result<CategoryDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public CreateCategoryCommandHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result<CategoryDto>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            var categoryRepo = _unitOfWork.GetRepository<CategoryEntity, Guid>();

            if (request.ParentCategoryId.HasValue)
            {
                var parentExists = await categoryRepo.ExistsAsync(
                    c => !c.IsDeleted && c.Id == request.ParentCategoryId.Value,
                    cancellationToken);

                if (!parentExists)
                {
                    return Result<CategoryDto>.BadRequest(LocalizationKeys.Category.ParentCategoryNotFound);
                }
            }

            var currentUserId = _currentUserService.UserId != Guid.Empty
                ? _currentUserService.UserId.ToString()
                : "System";

            var category = CategoryEntity.Create(
                request.NameEn.Trim(),
                request.NameAr.Trim(),
                request.Description,
                request.ImageName,
                request.ParentCategoryId,
                currentUserId);

            await categoryRepo.AddAsync(category, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<CategoryDto>.Created(ToDto(category), LocalizationKeys.Category.Created);
        }

        private static CategoryDto ToDto(CategoryEntity category)
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
