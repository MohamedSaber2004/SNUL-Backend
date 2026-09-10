using MediatR;
using Microsoft.EntityFrameworkCore;
using SNUL.Shared.Common.DTOs.UserManagement;
using SNUL.Shared.Common.Repositories.Interfaces.Base;
using SNUL.Shared.Domain.Models;
using SNUL.Shared.Localization;
using SNUL.Shared.Results;

namespace UserManagement.Service.API.Features.Cities.Queries.GetCities
{
    public class GetCitiesQueryHandler : IRequestHandler<GetCitiesQuery, Result<IReadOnlyList<CityDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetCitiesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<IReadOnlyList<CityDto>>> Handle(GetCitiesQuery request, CancellationToken cancellationToken)
        {
            var cityRepo = _unitOfWork.GetRepository<City, Guid>();
            var query = cityRepo.GetAllWithIncluding(c => !c.IsDeleted, c => c.Country);

            if (request.CountryId.HasValue)
            {
                query = query.Where(c => c.CountryId == request.CountryId.Value);
            }

            var cities = await query
                .OrderBy(c => c.NameEn)
                .Select(c => new CityDto
                {
                    Id = c.Id,
                    CountryId = c.CountryId,
                    CountryNameEn = c.Country != null ? c.Country.NameEn : null,
                    CountryNameAr = c.Country != null ? c.Country.NameAr : null,
                    NameEn = c.NameEn,
                    NameAr = c.NameAr,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt
                })
                .ToListAsync(cancellationToken);

            return Result<IReadOnlyList<CityDto>>.Success(cities, LocalizationKeys.City.ListFetched);
        }
    }
}
