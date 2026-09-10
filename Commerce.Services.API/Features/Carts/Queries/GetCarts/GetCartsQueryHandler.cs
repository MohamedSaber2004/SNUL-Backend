using MediatR;
using Microsoft.EntityFrameworkCore;
using SNUL.Shared.Common.DTOs.Commerce;
using SNUL.Shared.Common.Extensions;
using SNUL.Shared.Common.Repositories.Interfaces.Base;
using SNUL.Shared.Localization;
using SNUL.Shared.Results;
using CartEntity = SNUL.Shared.Domain.Models.Cart;
namespace Commerce.Services.API.Features.Carts.Queries.GetCarts
{
    public class GetCartsQueryHandler : IRequestHandler<GetCartsQuery, PaginatedResult<CartDto>>
    {
        private readonly IUnitOfWork _uow;
        public GetCartsQueryHandler(IUnitOfWork uow) => _uow = uow;
        public async Task<PaginatedResult<CartDto>> Handle(GetCartsQuery r, CancellationToken ct)
        {
            var repo = _uow.GetRepository<CartEntity, Guid>();
            var q = repo.GetAll(c => !c.IsDeleted).AsNoTracking();
            return await q.OrderByDescending(c => c.CreatedAt).ToPaginatedListAsync(c => new CartDto { Id = c.Id, UserId = c.UserId, SessionId = c.SessionId, CurrencyId = c.CurrencyId, IsActive = c.IsActive, CreatedAt = c.CreatedAt }, r.PageNumber, r.PageSize, LocalizationKeys.Cart.ListFetched, ct);
        }
    }
}
