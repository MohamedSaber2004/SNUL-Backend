using MediatR;
using SNUL.Shared.Common.Interfaces;
using SNUL.Shared.Common.Repositories.Interfaces.Base;
using SNUL.Shared.Localization;
using SNUL.Shared.Results;
using Sales.Services.API.Features.Shared;
using QuoteEntity = SNUL.Shared.Domain.Models.Quote;
using RFQEntity = SNUL.Shared.Domain.Models.RFQ;
namespace Sales.Services.API.Features.Quotes.Commands.DeclineQuote
{
    public class DeclineQuoteCommand : IRequest<Result<string>> { public Guid Id { get; set; } }
    public class DeclineQuoteCommandHandler : IRequestHandler<DeclineQuoteCommand, Result<string>>
    {
        private readonly IUnitOfWork _uow; private readonly ICurrentUserService _cur;
        public DeclineQuoteCommandHandler(IUnitOfWork uow, ICurrentUserService cur) { _uow = uow; _cur = cur; }
        public async Task<Result<string>> Handle(DeclineQuoteCommand r, CancellationToken ct)
        {
            var repo = _uow.GetRepository<QuoteEntity, Guid>();
            var q = await repo.GetByIdAsync(r.Id, ct);
            if (q == null || q.IsDeleted) return Result<string>.NotFound(LocalizationKeys.Quote.NotFound);
            // Organization users may only decide quotes issued to their own company.
            var caller = await BuyerScope.GetAsync(_uow, _cur, ct);
            if (caller.IsOrganizationUser && !await OwnsQuoteAsync(q.RFQId, caller.CompanyId, ct)) return Result<string>.NotFound(LocalizationKeys.Quote.NotFound);
            q.Status = SNUL.Shared.Domain.Models.QuoteStatus.Declined; q.MarkAsUpdated("System"); repo.Update(q);
            // A declined quote re-opens the parent RFQ so it can be re-priced.
            if (q.RFQId.HasValue)
            {
                var rfqRepo = _uow.GetRepository<RFQEntity, Guid>();
                var rfq = await rfqRepo.GetByIdAsync(q.RFQId.Value, ct);
                if (rfq != null && !rfq.IsDeleted && rfq.Status == SNUL.Shared.Domain.Models.RFQStatus.Quoted)
                {
                    rfq.Status = SNUL.Shared.Domain.Models.RFQStatus.Pending;
                    rfq.MarkAsUpdated("System");
                }
            }
            await _uow.SaveChangesAsync(ct);
            return Result<string>.Success(q.Id.ToString(), LocalizationKeys.Quote.Declined);
        }

        private async Task<bool> OwnsQuoteAsync(Guid? rfqId, Guid? companyId, CancellationToken ct)
        {
            if (!rfqId.HasValue || !companyId.HasValue) return false;
            var rfqRepo = _uow.GetRepository<RFQEntity, Guid>();
            var rfq = await rfqRepo.GetByIdAsync(rfqId.Value, ct);
            return rfq != null && !rfq.IsDeleted && rfq.CompanyId == companyId.Value;
        }
    }
}
