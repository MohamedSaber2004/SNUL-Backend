using MediatR;
using SNUL.Shared.Common.Interfaces;
using SNUL.Shared.Common.Repositories.Interfaces.Base;
using SNUL.Shared.Domain.Models;
using SNUL.Shared.Localization;
using SNUL.Shared.Results;

namespace UserManagement.Service.API.Features.Zones.Commands.DeleteZone
{
    public class DeleteZoneCommandHandler : IRequestHandler<DeleteZoneCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public DeleteZoneCommandHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result<string>> Handle(DeleteZoneCommand request, CancellationToken cancellationToken)
        {
            var zoneRepo = _unitOfWork.GetRepository<Zone, Guid>();
            var zone = await zoneRepo.GetByIdAsync(request.Id, cancellationToken);
            if (zone == null || zone.IsDeleted)
            {
                return Result<string>.NotFound(LocalizationKeys.Zone.NotFound);
            }

            var currentUserId = _currentUserService.UserId != Guid.Empty
                ? _currentUserService.UserId.ToString()
                : "System";

            zone.MarkAsDeleted(currentUserId);
            zoneRepo.Update(zone);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<string>.Success(zone.Id.ToString(), LocalizationKeys.Zone.Deleted);
        }
    }
}
