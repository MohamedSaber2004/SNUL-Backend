using MediatR;
using SNUL.Shared.Common.DTOs.Sales;
using SNUL.Shared.Results;
namespace Sales.Services.API.Features.Quotes.Queries.GetQuotes
{
    public class GetQuotesQuery : IRequest<PaginatedResult<QuoteDto>> { public int PageNumber { get; set; } = 1; public int PageSize { get; set; } = 10; }
}
