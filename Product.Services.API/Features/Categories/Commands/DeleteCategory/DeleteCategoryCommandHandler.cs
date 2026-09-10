using MediatR;
using SNUL.Shared.Common.Interfaces;
using SNUL.Shared.Common.Repositories.Interfaces.Base;
using SNUL.Shared.Localization;
using SNUL.Shared.Results;
using CategoryEntity = SNUL.Shared.Domain.Models.Category;

namespace Product.Services.API.Features.Categories.Commands.DeleteCategory
{
    public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public DeleteCategoryCommandHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result<string>> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            var categoryRepo = _unitOfWork.GetRepository<CategoryEntity, Guid>();
            var category = await categoryRepo.GetByIdAsync(request.Id, cancellationToken);

            if (category == null || category.IsDeleted)
            {
                return Result<string>.NotFound(LocalizationKeys.Category.NotFound);
            }

            var currentUserId = _currentUserService.UserId != Guid.Empty
                ? _currentUserService.UserId.ToString()
                : "System";

            category.MarkAsDeleted(currentUserId);
            categoryRepo.Update(category);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<string>.Success(category.Id.ToString(), LocalizationKeys.Category.Deleted);
        }
    }
}
