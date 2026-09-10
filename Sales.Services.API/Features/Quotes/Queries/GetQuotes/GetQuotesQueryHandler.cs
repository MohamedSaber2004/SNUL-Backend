using MediatR;
using Microsoft.EntityFrameworkCore;
using SNUL.Shared.Common.DTOs.Sales;
using SNUL.Shared.Common.Extensions;
using SNUL.Shared.Common.Interfaces;
using SNUL.Shared.Common.Repositories.Interfaces.Base;
using SNUL.Shared.Localization;
using SNUL.Shared.Results;
using Sales.Services.API.Features.Shared;
using QuoteEntity = SNUL.Shared.Domain.Models.Quote;
namespace Sales.Services.API.Features.Quotes.Queries.GetQuotes
{
    public class GetQuotesQueryHandler : IRequestHandler<GetQuotesQuery, PaginatedResult<QuoteDto>>
    {
        private readonly IUnitOfWork _uow; private readonly ICurrentUserService _cur;
        public GetQuotesQueryHandler(IUnitOfWork uow, ICurrentUserService cur) { _uow = uow; _cur = cur; }
        public async Task<PaginatedResult<QuoteDto>> Handle(GetQuotesQuery r, CancellationToken ct)
        {
            var repo = _uow.GetRepository<QuoteEntity, Guid>();
            var q = repo.GetAll(x => !x.IsDeleted).AsNoTracking();
            
            var caller = await BuyerScope.GetAsync(_uow, _cur, ct);
            if (caller.IsOrganizationUser) q = q.Where(x => x.RFQ != null && x.RFQ.CompanyId == caller.CompanyId);
            return await q.OrderByDescending(x => x.CreatedAt).ToPaginatedListAsync(x => new QuoteDto { Id = x.Id, QuoteNumber = x.QuoteNumber, RFQId = x.RFQId, Amount = x.Amount, ValidUntil = x.ValidUntil, Status = x.Status.ToString(), CreatedAt = x.CreatedAt }, r.PageNumber, r.PageSize, LocalizationKeys.Quote.ListFetched, ct);
        }
    }
}
