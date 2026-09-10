using MediatR;
using SNUL.Shared.Common.DTOs.Products;
using SNUL.Shared.Results;

namespace Product.Services.API.Features.Currencies.Queries.GetCurrencyById
{
    public class GetCurrencyByIdQuery : IRequest<Result<CurrencyDto>>
    {
        public Guid Id { get; set; }
    }
}
