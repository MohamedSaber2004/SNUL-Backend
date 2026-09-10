using MediatR;
using SNUL.Shared.Common.DTOs.Sales;
using SNUL.Shared.Results;
namespace Sales.Services.API.Features.Quotes.Queries.GetQuoteById
{
    public class GetQuoteByIdQuery : IRequest<Result<QuoteDto>> { public Guid Id { get; set; } }
}
