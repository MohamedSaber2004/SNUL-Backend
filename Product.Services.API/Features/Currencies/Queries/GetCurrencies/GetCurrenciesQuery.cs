using MediatR;
using SNUL.Shared.Common.DTOs.Products;
using SNUL.Shared.Results;

namespace Product.Services.API.Features.Currencies.Queries.GetCurrencies
{
    public class GetCurrenciesQuery : IRequest<PaginatedResult<CurrencyDto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 50;
        public string? SearchTerm { get; set; }
        public bool? IsActive { get; set; }
    }
}
