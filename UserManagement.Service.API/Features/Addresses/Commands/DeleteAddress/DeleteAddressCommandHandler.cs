using MediatR;
using SNUL.Shared.Common.Interfaces;
using SNUL.Shared.Common.Repositories.Interfaces.Base;
using SNUL.Shared.Domain.Models;
using SNUL.Shared.Localization;
using SNUL.Shared.Results;

namespace UserManagement.Service.API.Features.Addresses.Commands.DeleteAddress
{
    public class DeleteAddressCommandHandler : IRequestHandler<DeleteAddressCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public DeleteAddressCommandHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result<string>> Handle(DeleteAddressCommand request, CancellationToken cancellationToken)
        {
            var addressRepo = _unitOfWork.GetRepository<UserAddress, Guid>();
            var address = await addressRepo.GetByIdAsync(request.Id, cancellationToken);
            if (address == null || address.IsDeleted)
            {
                return Result<string>.NotFound(LocalizationKeys.UserAddress.AddressNotFound);
            }

            var wasDefault = address.IsDefault;
            var currentUserId = _currentUserService.UserId != Guid.Empty
                ? _currentUserService.UserId.ToString()
                : "System";

            address.MarkAsDeleted(currentUserId);
            addressRepo.Update(address);

if (wasDefault)
            {
                var remaining = await addressRepo.GetAllListAsync(a => a.UserId == address.UserId && a.Id != address.Id && !a.IsDeleted, cancellationToken);
                var next = remaining.OrderByDescending(a => a.CreatedAt).FirstOrDefault();
                if (next != null)
                {
                    next.IsDefault = true;
                    next.MarkAsUpdated(currentUserId);
                }
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<string>.Success(address.Id.ToString(), LocalizationKeys.UserAddress.AddressDeleted);
        }
    }
}
