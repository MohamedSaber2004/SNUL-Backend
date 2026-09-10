using MediatR;
using Microsoft.EntityFrameworkCore;
using SNUL.Shared.Common.DTOs.UserManagement;
using SNUL.Shared.Common.Repositories.Interfaces.Base;
using SNUL.Shared.Domain.Models;
using SNUL.Shared.Localization;
using SNUL.Shared.Results;

namespace UserManagement.Service.API.Features.Zones.Queries.GetZones
{
    public class GetZonesQueryHandler : IRequestHandler<GetZonesQuery, Result<IReadOnlyList<ZoneDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetZonesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<IReadOnlyList<ZoneDto>>> Handle(GetZonesQuery request, CancellationToken cancellationToken)
        {
            var zoneRepo = _unitOfWork.GetRepository<Zone, Guid>();
            var query = zoneRepo.GetAllWithIncluding(z => !z.IsDeleted, z => z.City);

            if (request.CityId.HasValue)
            {
                query = query.Where(z => z.CityId == request.CityId.Value);
            }

            var zones = await query
                .OrderBy(z => z.NameEn)
                .Select(z => new ZoneDto
                {
                    Id = z.Id,
                    CityId = z.CityId,
                    CityNameEn = z.City != null ? z.City.NameEn : null,
                    CityNameAr = z.City != null ? z.City.NameAr : null,
                    NameEn = z.NameEn,
                    NameAr = z.NameAr,
                    IsActive = z.IsActive,
                    CreatedAt = z.CreatedAt
                })
                .ToListAsync(cancellationToken);

            return Result<IReadOnlyList<ZoneDto>>.Success(zones, LocalizationKeys.Zone.ListFetched);
        }
    }
}
