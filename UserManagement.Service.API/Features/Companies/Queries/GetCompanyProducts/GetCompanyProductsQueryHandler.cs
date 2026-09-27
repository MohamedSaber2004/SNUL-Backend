using MediatR;
using SNUL.Shared.Common.DTOs.Products;
using SNUL.Shared.Common.Extensions;
using SNUL.Shared.Common.Repositories.Interfaces.Base;
using SNUL.Shared.Domain.Models;
using SNUL.Shared.Localization;
using SNUL.Shared.Results;

namespace UserManagement.Service.API.Features.Companies.Queries.GetCompanyProducts
{
    public class GetCompanyProductsQueryHandler : IRequestHandler<GetCompanyProductsQuery, PaginatedResult<ProductDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetCompanyProductsQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<PaginatedResult<ProductDto>> Handle(GetCompanyProductsQuery request, CancellationToken cancellationToken)
        {
            var productRepo = _unitOfWork.GetRepository<Product, Guid>();
            var query = productRepo.GetAll(p => !p.IsDeleted && p.IsActive && p.CompanyId == request.CompanyId);

            if (request.CategoryId.HasValue)
            {
                var targetCatId = request.CategoryId.Value;
                query = query.Where(p => p.CategoryId == targetCatId || (p.Category != null && p.Category.ParentCategoryId == targetCatId));
            }

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var term = request.SearchTerm.Trim().ToLower();
                query = query.Where(p =>
                    p.NameEn.ToLower().Contains(term) ||
                    p.NameAr.ToLower().Contains(term) ||
                    p.Sku.ToLower().Contains(term) ||
                    (p.Material != null && p.Material.ToLower().Contains(term)));
            }

            if (!string.IsNullOrWhiteSpace(request.Sku))
                query = query.Where(p => p.Sku.ToLower().Contains(request.Sku.Trim().ToLower()));

            query = query.OrderByDescending(p => p.CreatedAt);

            return await query.ToPaginatedListAsync(p => new ProductDto
            {
                Id = p.Id,
                NameEn = p.NameEn,
                NameAr = p.NameAr,
                Sku = p.Sku,
                Slug = p.Slug,
                Description = p.Description,
                Price = p.Price,
                Stock = p.Stock,
                Specifications = p.Specifications,
                ImageName = p.ImageName,
                Material = p.Material,
                LengthCm = p.LengthCm,
                CurrencyId = p.CurrencyId,
                CurrencyCode = p.Currency != null ? p.Currency.Code : null,
                CurrencySymbol = p.Currency != null ? p.Currency.Symbol : null,
                CategoryId = p.CategoryId,
                CategoryNameEn = p.Category != null ? p.Category.NameEn : null,
                CategoryNameAr = p.Category != null ? p.Category.NameAr : null,
                CompanyId = p.CompanyId,
                CompanyName = p.Company != null ? p.Company.Name : null,
                IsActive = p.IsActive,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            }, request.PageNumber, request.PageSize, LocalizationKeys.Product.ListFetched, cancellationToken);
        }
    }
}
