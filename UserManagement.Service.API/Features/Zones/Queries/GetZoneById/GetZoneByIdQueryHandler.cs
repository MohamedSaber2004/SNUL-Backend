using MediatR;
using Microsoft.EntityFrameworkCore;
using SNUL.Shared.Common.DTOs.UserManagement;
using SNUL.Shared.Common.Repositories.Interfaces.Base;
using SNUL.Shared.Domain.Models;
using SNUL.Shared.Localization;
using SNUL.Shared.Results;

namespace UserManagement.Service.API.Features.Zones.Queries.GetZoneById
{
    public class GetZoneByIdQueryHandler : IRequestHandler<GetZoneByIdQuery, Result<ZoneDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetZoneByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<ZoneDto>> Handle(GetZoneByIdQuery request, CancellationToken cancellationToken)
        {
            var zoneRepo = _unitOfWork.GetRepository<Zone, Guid>();
            var zone = await zoneRepo
                .GetAllWithIncluding(z => z.Id == request.Id && !z.IsDeleted, z => z.City)
                .FirstOrDefaultAsync(cancellationToken);

            if (zone == null)
            {
                return Result<ZoneDto>.NotFound(LocalizationKeys.Zone.NotFound);
            }

            var dto = new ZoneDto
            {
                Id = zone.Id,
                CityId = zone.CityId,
                CityNameEn = zone.City?.NameEn,
                CityNameAr = zone.City?.NameAr,
                NameEn = zone.NameEn,
                NameAr = zone.NameAr,
                IsActive = zone.IsActive,
                CreatedAt = zone.CreatedAt
            };

            return Result<ZoneDto>.Success(dto, LocalizationKeys.Zone.Fetched);
        }
    }
}
