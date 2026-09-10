using MediatR;
using SNUL.Shared.Common.Interfaces;
using SNUL.Shared.Common.Repositories.Interfaces.Base;
using SNUL.Shared.Localization;
using SNUL.Shared.Results;
using OrderEntity = SNUL.Shared.Domain.Models.Order;

namespace Commerce.Services.API.Features.Orders.Commands.UpdateOrderStatus
{
    public class UpdateOrderStatusCommandHandler : IRequestHandler<UpdateOrderStatusCommand, Result<string>>
    {
        private readonly IUnitOfWork _uow;
        private readonly ICurrentUserService _currentUser;

        public UpdateOrderStatusCommandHandler(IUnitOfWork uow, ICurrentUserService currentUser)
        {
            _uow = uow;
            _currentUser = currentUser;
        }

        public async Task<Result<string>> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
        {
            var repo = _uow.GetRepository<OrderEntity, Guid>();
            var order = await repo.GetByIdAsync(request.Id, cancellationToken);
            if (order == null || order.IsDeleted)
                return Result<string>.NotFound(LocalizationKeys.Order.NotFound);

            if (!Enum.TryParse<SNUL.Shared.Domain.Models.OrderStatus>(request.Status, true, out var status))
                return Result<string>.BadRequest(LocalizationKeys.Order.InvalidStatus);

            var currentUserId = _currentUser.UserId != Guid.Empty ? _currentUser.UserId.ToString() : "System";
            order.Status = status;
            order.MarkAsUpdated(currentUserId);
            repo.Update(order);
            await _uow.SaveChangesAsync(cancellationToken);

            return Result<string>.Success(order.Id.ToString(), LocalizationKeys.Order.Updated);
        }
    }
}
