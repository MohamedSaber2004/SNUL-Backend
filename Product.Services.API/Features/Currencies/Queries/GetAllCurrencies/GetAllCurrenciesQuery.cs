using MediatR;
using SNUL.Shared.Common.DTOs.Products;
using SNUL.Shared.Results;

namespace Product.Services.API.Features.Currencies.Queries.GetAllCurrencies
{
    /// <summary>Unpaginated currency list, for currency switchers and FX lookups.</summary>
    public class GetAllCurrenciesQuery : IRequest<Result<List<CurrencyDto>>>
    {
    }
}
