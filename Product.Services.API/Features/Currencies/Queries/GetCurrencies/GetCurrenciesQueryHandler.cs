using MediatR;
using Microsoft.EntityFrameworkCore;
using SNUL.Shared.Common.DTOs.Products;
using SNUL.Shared.Common.Extensions;
using SNUL.Shared.Common.Repositories.Interfaces.Base;
using SNUL.Shared.Localization;
using SNUL.Shared.Results;
using CurrencyEntity = SNUL.Shared.Domain.Models.Currency;

namespace Product.Services.API.Features.Currencies.Queries.GetCurrencies
{
    public class GetCurrenciesQueryHandler : IRequestHandler<GetCurrenciesQuery, PaginatedResult<CurrencyDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetCurrenciesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PaginatedResult<CurrencyDto>> Handle(GetCurrenciesQuery request, CancellationToken cancellationToken)
        {
            var currencyRepo = _unitOfWork.GetRepository<CurrencyEntity, Guid>();
            var query = currencyRepo.GetAll(c => !c.IsDeleted).AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var term = request.SearchTerm.Trim().ToLower();
                query = query.Where(c =>
                    c.NameEn.ToLower().Contains(term) ||
                    c.NameAr.ToLower().Contains(term) ||
                    c.Code.ToLower().Contains(term));
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(c => c.IsActive == request.IsActive.Value);
            }

            return await query
                .OrderBy(c => c.Code)
                .ToPaginatedListAsync(
                    c => new CurrencyDto
                    {
                        Id = c.Id,
                        NameEn = c.NameEn,
                        NameAr = c.NameAr,
                        Code = c.Code,
                        Symbol = c.Symbol,
                        SymbolNative = c.SymbolNative,
                        DecimalDigits = c.DecimalDigits,
                        IsActive = c.IsActive,
                        CreatedAt = c.CreatedAt,
                        UpdatedAt = c.UpdatedAt
                    },
                    request.PageNumber,
                    request.PageSize,
                    LocalizationKeys.Currency.ListFetched,
                    cancellationToken);
        }
    }
}
