using Commerce.Services.API.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SNUL.Shared.Common.DTOs.Commerce;
using SNUL.Shared.Common.Interfaces;
using SNUL.Shared.Common.Repositories.Interfaces.Base;
using SNUL.Shared.Domain.Models;
using SNUL.Shared.Enums;
using SNUL.Shared.Localization;
using SNUL.Shared.Results;
using OrderEntity = SNUL.Shared.Domain.Models.Order;

namespace Commerce.Services.API.Features.Orders.Queries.GetOrderById
{
    public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, Result<OrderDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly ICurrentUserService _currentUser;

        public GetOrderByIdQueryHandler(IUnitOfWork uow, ICurrentUserService currentUser)
        {
            _uow = uow;
            _currentUser = currentUser;
        }

        public async Task<Result<OrderDto>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            var repo = _uow.GetRepository<OrderEntity, Guid>();
            var order = await repo.GetAll(o => !o.IsDeleted && o.Id == request.Id)
                .Include(o => o.Currency)
                .Include(o => o.Items.Where(i => !i.IsDeleted))
                    .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(cancellationToken);

            if (order == null)
                return Result<OrderDto>.NotFound(LocalizationKeys.Order.NotFound);

            if (_currentUser.UserId != Guid.Empty)
            {
                var userRepo = _uow.GetRepository<ApplicationUser, Guid>();
                var user = await userRepo.GetByIdAsync(_currentUser.UserId, cancellationToken);
                if (user != null && !user.IsDeleted && user.UserType == UserType.OrganizationUser)
                {
                    var isOwner = order.UserId == user.Id || (user.CompanyId.HasValue && order.CompanyId == user.CompanyId.Value);
                    if (!isOwner) return Result<OrderDto>.NotFound(LocalizationKeys.Order.NotFound);
                }
            }

            var dto = CommerceDtoMapper.ToDto(order);
            return Result<OrderDto>.Success(dto, LocalizationKeys.Order.Fetched);
        }
    }
}
