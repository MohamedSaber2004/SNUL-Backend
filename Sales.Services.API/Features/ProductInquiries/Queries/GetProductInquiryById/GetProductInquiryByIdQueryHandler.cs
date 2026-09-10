using MediatR;
using Microsoft.EntityFrameworkCore;
using SNUL.Shared.Common.DTOs.Sales;
using SNUL.Shared.Common.Repositories.Interfaces.Base;
using SNUL.Shared.Domain.Models;
using SNUL.Shared.Localization;
using SNUL.Shared.Results;
namespace Sales.Services.API.Features.ProductInquiries.Queries.GetProductInquiryById
{
    public class GetProductInquiryByIdQueryHandler : IRequestHandler<GetProductInquiryByIdQuery, Result<ProductInquiryDto>>
    {
        private readonly IUnitOfWork _uow;
        public GetProductInquiryByIdQueryHandler(IUnitOfWork uow) => _uow = uow;
        public async Task<Result<ProductInquiryDto>> Handle(GetProductInquiryByIdQuery r, CancellationToken ct)
        {
            var repo = _uow.GetRepository<ProductInquiry, Guid>();
            var x = await repo.GetAll(i => i.Id == r.Id && !i.IsDeleted).Include(i => i.Product).FirstOrDefaultAsync(ct);
            if (x == null) return Result<ProductInquiryDto>.NotFound(LocalizationKeys.ProductInquiry.NotFound);
            return Result<ProductInquiryDto>.Success(new ProductInquiryDto { Id = x.Id, ProductId = x.ProductId, ProductNameEn = x.Product != null ? x.Product.NameEn : null, ProductNameAr = x.Product != null ? x.Product.NameAr : null, ProductSku = x.Product != null ? x.Product.Sku : null, Name = x.Name, Organization = x.Organization, Message = x.Message, Email = x.Email, CreatedAt = x.CreatedAt }, LocalizationKeys.ProductInquiry.Fetched);
        }
    }
}
