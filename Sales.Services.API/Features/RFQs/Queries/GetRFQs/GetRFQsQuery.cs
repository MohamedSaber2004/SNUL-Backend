using MediatR;
using SNUL.Shared.Results;
using SNUL.Shared.Common.DTOs.Sales;
namespace Sales.Services.API.Features.RFQs.Queries.GetRFQs
{
    public class GetRFQsQuery : IRequest<PaginatedResult<RFQDto>> { public int PageNumber { get; set; } = 1; public int PageSize { get; set; } = 10; }
}
