using MediatR;
using SNUL.Shared.Common.DTOs.UserManagement;
using SNUL.Shared.Common.Interfaces;
using SNUL.Shared.Common.Repositories.Interfaces.Base;
using SNUL.Shared.Domain.Models;
using SNUL.Shared.Localization;
using SNUL.Shared.Results;

namespace UserManagement.Service.API.Features.Zones.Commands.CreateZone
{
    public class CreateZoneCommandHandler : IRequestHandler<CreateZoneCommand, Result<ZoneDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public CreateZoneCommandHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result<ZoneDto>> Handle(CreateZoneCommand request, CancellationToken cancellationToken)
        {
            var cityRepo = _unitOfWork.GetRepository<City, Guid>();
            var city = await cityRepo.GetByIdAsync(request.CityId, cancellationToken);
            if (city == null || city.IsDeleted)
            {
                return Result<ZoneDto>.NotFound(LocalizationKeys.City.NotFound);
            }

            var zoneRepo = _unitOfWork.GetRepository<Zone, Guid>();
            var exists = await zoneRepo.ExistsAsync(
                z => !z.IsDeleted && z.CityId == request.CityId &&
                     (z.NameEn.ToLower() == request.NameEn.Trim().ToLower() || z.NameAr == request.NameAr.Trim()),
                cancellationToken);

            if (exists)
            {
                return Result<ZoneDto>.Conflict(LocalizationKeys.Zone.AlreadyExists);
            }

            var currentUserId = _currentUserService.UserId != Guid.Empty
                ? _currentUserService.UserId.ToString()
                : "System";

            var zone = Zone.Create(
                request.CityId,
                request.NameEn.Trim(),
                request.NameAr.Trim(),
                currentUserId);

            await zoneRepo.AddAsync(zone, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var dto = new ZoneDto
            {
                Id = zone.Id,
                CityId = zone.CityId,
                CityNameEn = city.NameEn,
                CityNameAr = city.NameAr,
                NameEn = zone.NameEn,
                NameAr = zone.NameAr,
                IsActive = zone.IsActive,
                CreatedAt = zone.CreatedAt
            };

            return Result<ZoneDto>.Created(dto, LocalizationKeys.Zone.Created);
        }
    }
}
