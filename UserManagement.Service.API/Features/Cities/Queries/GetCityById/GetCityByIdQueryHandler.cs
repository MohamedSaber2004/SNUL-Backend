using MediatR;
using Microsoft.EntityFrameworkCore;
using SNUL.Shared.Common.DTOs.UserManagement;
using SNUL.Shared.Common.Repositories.Interfaces.Base;
using SNUL.Shared.Domain.Models;
using SNUL.Shared.Localization;
using SNUL.Shared.Results;

namespace UserManagement.Service.API.Features.Cities.Queries.GetCityById
{
    public class GetCityByIdQueryHandler : IRequestHandler<GetCityByIdQuery, Result<CityDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetCityByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<CityDto>> Handle(GetCityByIdQuery request, CancellationToken cancellationToken)
        {
            var cityRepo = _unitOfWork.GetRepository<City, Guid>();
            var city = await cityRepo
                .GetAllWithIncluding(c => c.Id == request.Id && !c.IsDeleted, c => c.Country)
                .FirstOrDefaultAsync(cancellationToken);

            if (city == null)
            {
                return Result<CityDto>.NotFound(LocalizationKeys.City.NotFound);
            }

            var dto = new CityDto
            {
                Id = city.Id,
                CountryId = city.CountryId,
                CountryNameEn = city.Country?.NameEn,
                CountryNameAr = city.Country?.NameAr,
                NameEn = city.NameEn,
                NameAr = city.NameAr,
                IsActive = city.IsActive,
                CreatedAt = city.CreatedAt
            };

            return Result<CityDto>.Success(dto, LocalizationKeys.City.Fetched);
        }
    }
}
