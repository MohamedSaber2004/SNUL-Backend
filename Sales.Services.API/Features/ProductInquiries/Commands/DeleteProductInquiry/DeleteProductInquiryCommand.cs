using MediatR;
using SNUL.Shared.Results;
namespace Sales.Services.API.Features.ProductInquiries.Commands.DeleteProductInquiry
{
    public class DeleteProductInquiryCommand : IRequest<Result<string>> { public Guid Id { get; set; } }
}
