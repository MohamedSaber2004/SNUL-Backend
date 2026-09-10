using MediatR;
using SNUL.Shared.Common.DTOs.Sales;
using SNUL.Shared.Results;
namespace Sales.Services.API.Features.ProductInquiries.Queries.GetProductInquiryById
{
    public class GetProductInquiryByIdQuery : IRequest<Result<ProductInquiryDto>> { public Guid Id { get; set; } }
}
