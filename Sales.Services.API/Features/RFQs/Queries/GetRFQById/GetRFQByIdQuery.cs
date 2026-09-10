using MediatR;
using SNUL.Shared.Common.DTOs.Sales;
using SNUL.Shared.Results;
namespace Sales.Services.API.Features.RFQs.Queries.GetRFQById
{
    public class GetRFQByIdQuery : IRequest<Result<RFQDto>> { public Guid Id { get; set; } }
}
