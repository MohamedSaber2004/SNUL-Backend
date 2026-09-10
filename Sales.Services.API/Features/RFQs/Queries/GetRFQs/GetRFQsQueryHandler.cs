using MediatR;
using Microsoft.EntityFrameworkCore;
using SNUL.Shared.Common.DTOs.Sales;
using SNUL.Shared.Common.Extensions;
using SNUL.Shared.Common.Interfaces;
using SNUL.Shared.Common.Repositories.Interfaces.Base;
using SNUL.Shared.Localization;
using SNUL.Shared.Results;
using Sales.Services.API.Features.Shared;
using RFQEntity = SNUL.Shared.Domain.Models.RFQ;
namespace Sales.Services.API.Features.RFQs.Queries.GetRFQs
{
    public class GetRFQsQueryHandler : IRequestHandler<GetRFQsQuery, PaginatedResult<RFQDto>>
    {
        private readonly IUnitOfWork _uow; private readonly ICurrentUserService _cur;
        public GetRFQsQueryHandler(IUnitOfWork uow, ICurrentUserService cur) { _uow = uow; _cur = cur; }
        public async Task<PaginatedResult<RFQDto>> Handle(GetRFQsQuery r, CancellationToken ct)
        {
            var repo = _uow.GetRepository<RFQEntity, Guid>();
            var q = repo.GetAll(x => !x.IsDeleted).AsNoTracking();
            
            var caller = await BuyerScope.GetAsync(_uow, _cur, ct);
            if (caller.IsOrganizationUser) q = q.Where(x => x.CompanyId == caller.CompanyId);
            return await q.OrderByDescending(x => x.CreatedAt).ToPaginatedListAsync(x => new RFQDto { Id = x.Id, RFQNumber = x.RFQNumber, CompanyId = x.CompanyId, Status = x.Status.ToString(), AssignedSalesRepId = x.AssignedSalesRepId, CreatedAt = x.CreatedAt, Items = x.Items.Where(i => !i.IsDeleted).Select(i => new RFQItemDto { Id = i.Id, RFQId = i.RFQId, ProductId = i.ProductId, ProductNameEn = i.Product != null ? i.Product.NameEn : null, ProductNameAr = i.Product != null ? i.Product.NameAr : null, Quantity = i.Quantity, UnitPrice = i.UnitPrice, Notes = i.Notes }).ToList() }, r.PageNumber, r.PageSize, LocalizationKeys.RFQ.ListFetched, ct);
        }
    }
}
