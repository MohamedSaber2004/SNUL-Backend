using MediatR;
using Microsoft.EntityFrameworkCore;
using SNUL.Shared.Common.DTOs.Products;
using SNUL.Shared.Common.Repositories.Interfaces.Base;
using SNUL.Shared.Localization;
using SNUL.Shared.Results;
using CurrencyEntity = SNUL.Shared.Domain.Models.Currency;

namespace Product.Services.API.Features.Currencies.Queries.GetCurrencyByCode
{
    public class GetCurrencyByCodeQueryHandler : IRequestHandler<GetCurrencyByCodeQuery, Result<CurrencyDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetCurrencyByCodeQueryHandler(IUnitOfWork uow) => _unitOfWork = uow;

        public async Task<Result<CurrencyDto>> Handle(GetCurrencyByCodeQuery request, CancellationToken ct)
        {
            var repo = _unitOfWork.GetRepository<CurrencyEntity, Guid>();
            var code = request.Code.Trim().ToUpperInvariant();
            var cur = await repo.GetAll(c => !c.IsDeleted && c.Code == code).FirstOrDefaultAsync(ct);
            if (cur == null) return Result<CurrencyDto>.NotFound(LocalizationKeys.Currency.NotFound);
            return Result<CurrencyDto>.Success(new CurrencyDto
            {
                Id = cur.Id,
                NameEn = cur.NameEn,
                NameAr = cur.NameAr,
                Code = cur.Code,
                Symbol = cur.Symbol,
                SymbolNative = cur.SymbolNative,
                DecimalDigits = cur.DecimalDigits,
                IsActive = cur.IsActive,
                CreatedAt = cur.CreatedAt,
                UpdatedAt = cur.UpdatedAt
            }, LocalizationKeys.Currency.Fetched);
        }
    }
}
