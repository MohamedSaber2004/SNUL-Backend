using MediatR;
using SNUL.Shared.Common.DTOs.Sales;
using SNUL.Shared.Domain.Models;
using SNUL.Shared.Localization;
using SNUL.Shared.Persistance;
using SNUL.Shared.Results;

namespace Sales.Services.API.Features.ProductInquiries.Commands.CreateProductInquiry
{
    public class CreateProductInquiryCommandHandler : IRequestHandler<CreateProductInquiryCommand, Result<ProductInquiryDto>>
    {
        private readonly SnulDbContext _db;
        public CreateProductInquiryCommandHandler(SnulDbContext db) => _db = db;

        public async Task<Result<ProductInquiryDto>> Handle(CreateProductInquiryCommand request, CancellationToken ct)
        {
            var product = await _db.Products.FindAsync(new object[] { request.ProductId }, ct);
            if (product == null || product.IsDeleted)
                return Result<ProductInquiryDto>.NotFound(LocalizationKeys.Product.NotFound);

            var entity = new ProductInquiry
            {
                Id = Guid.NewGuid(),
                ProductId = request.ProductId,
                Name = request.Name.Trim(),
                Organization = request.Organization.Trim(),
                Message = request.Message.Trim(),
                Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim(),
            };
            entity.MarkAsCreated(request.Email ?? "Anonymous");
            _db.ProductInquiries.Add(entity);
            await _db.SaveChangesAsync(ct);
            var dto = new ProductInquiryDto { Id = entity.Id, ProductId = entity.ProductId, Name = entity.Name, Organization = entity.Organization, Message = entity.Message, Email = entity.Email, CreatedAt = entity.CreatedAt };
            return Result<ProductInquiryDto>.Success(dto, LocalizationKeys.ProductInquiry.Created, 201);
        }
    }
}
