using MediatR;
using Microsoft.EntityFrameworkCore;
using Product.Services.API.Common;
using SNUL.Shared.Common.DTOs.Products;
using SNUL.Shared.Common.Interfaces;
using SNUL.Shared.Common.Repositories.Interfaces.Base;
using SNUL.Shared.Localization;
using SNUL.Shared.Results;
using CategoryEntity = SNUL.Shared.Domain.Models.Category;
using CurrencyEntity = SNUL.Shared.Domain.Models.Currency;
using ProductEntity = SNUL.Shared.Domain.Models.Product;

namespace Product.Services.API.Features.Products.Commands.UpdateProduct
{
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Result<ProductDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public UpdateProductCommandHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result<ProductDto>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var productRepo = _unitOfWork.GetRepository<ProductEntity, Guid>();
            var product = await productRepo.GetByIdAsync(request.Id, cancellationToken);

            if (product == null || product.IsDeleted)
                return Result<ProductDto>.NotFound(LocalizationKeys.Product.NotFound);

            if (!string.Equals(product.Sku, request.Sku.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                var skuExists = await productRepo.ExistsAsync(p => !p.IsDeleted && p.Id != request.Id && p.Sku.ToLower() == request.Sku.Trim().ToLower(), cancellationToken);
                if (skuExists) return Result<ProductDto>.Conflict(LocalizationKeys.Product.SkuAlreadyExists);
            }
            if (!string.Equals(product.Slug, request.Slug.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                var slugExists = await productRepo.ExistsAsync(p => !p.IsDeleted && p.Id != request.Id && p.Slug.ToLower() == request.Slug.Trim().ToLower(), cancellationToken);
                if (slugExists) return Result<ProductDto>.Conflict(LocalizationKeys.Product.SlugAlreadyExists);
            }

            var categoryRepo = _unitOfWork.GetRepository<CategoryEntity, Guid>();
            var categoryExists = await categoryRepo.ExistsAsync(c => !c.IsDeleted && c.Id == request.CategoryId, cancellationToken);
            if (!categoryExists)
                return Result<ProductDto>.BadRequest(LocalizationKeys.Product.CategoryNotFound);

            if (request.CurrencyId.HasValue)
            {
                var currencyRepo = _unitOfWork.GetRepository<CurrencyEntity, Guid>();
                var currencyExists = await currencyRepo.ExistsAsync(c => !c.IsDeleted && c.Id == request.CurrencyId.Value, cancellationToken);
                if (!currencyExists)
                    return Result<ProductDto>.BadRequest(LocalizationKeys.Currency.NotFound);
            }

            var currentUserId = _currentUserService.UserId != Guid.Empty ? _currentUserService.UserId.ToString() : "System";

            product.Update(
                request.NameEn.Trim(),
                request.NameAr.Trim(),
                request.Sku.Trim(),
                request.Slug.Trim().ToLowerInvariant(),
                request.Description,
                request.Price,
                request.Stock,
                request.Specifications,
                request.ImageName,
                request.Material,
                request.LengthCm,
                request.CurrencyId,
                request.CategoryId,
                currentUserId);

            if (request.IsActive.HasValue)
                product.SetActiveState(request.IsActive.Value, currentUserId);

            productRepo.Update(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var dto = await productRepo.GetAll(p => !p.IsDeleted && p.Id == product.Id)
                .Select(ProductDtoMapper.Projection)
                .FirstOrDefaultAsync(cancellationToken);

            return Result<ProductDto>.Success(dto!, LocalizationKeys.Product.Updated);
        }
    }
}
