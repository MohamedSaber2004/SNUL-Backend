using MediatR;
using SNUL.Shared.Common.DTOs.Products;
using SNUL.Shared.Results;

namespace Product.Services.API.Features.Currencies.Queries.GetCurrencyByCode
{
    public class GetCurrencyByCodeQuery : IRequest<Result<CurrencyDto>>
    {
        public string Code { get; set; } = string.Empty;
    }
}
