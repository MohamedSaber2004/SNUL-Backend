using MediatR;
using SNUL.Shared.Common.DTOs.UserManagement;
using SNUL.Shared.Common.Interfaces;
using SNUL.Shared.Common.Repositories.Interfaces.Base;
using SNUL.Shared.Domain.Models;
using SNUL.Shared.Localization;
using SNUL.Shared.Results;

namespace UserManagement.Service.API.Features.Cities.Commands.CreateCity
{
    public class CreateCityCommandHandler : IRequestHandler<CreateCityCommand, Result<CityDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public CreateCityCommandHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result<CityDto>> Handle(CreateCityCommand request, CancellationToken cancellationToken)
        {
            var countryRepo = _unitOfWork.GetRepository<Country, Guid>();
            var country = await countryRepo.GetByIdAsync(request.CountryId, cancellationToken);
            if (country == null || country.IsDeleted)
            {
                return Result<CityDto>.NotFound(LocalizationKeys.Country.NotFound);
            }

            var cityRepo = _unitOfWork.GetRepository<City, Guid>();
            var exists = await cityRepo.ExistsAsync(
                c => !c.IsDeleted && c.CountryId == request.CountryId &&
                     (c.NameEn.ToLower() == request.NameEn.Trim().ToLower() || c.NameAr == request.NameAr.Trim()),
                cancellationToken);

            if (exists)
            {
                return Result<CityDto>.Conflict(LocalizationKeys.City.AlreadyExists);
            }

            var currentUserId = _currentUserService.UserId != Guid.Empty
                ? _currentUserService.UserId.ToString()
                : "System";

            var city = City.Create(
                request.CountryId,
                request.NameEn.Trim(),
                request.NameAr.Trim(),
                currentUserId);

            await cityRepo.AddAsync(city, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var dto = new CityDto
            {
                Id = city.Id,
                CountryId = city.CountryId,
                CountryNameEn = country.NameEn,
                CountryNameAr = country.NameAr,
                NameEn = city.NameEn,
                NameAr = city.NameAr,
                IsActive = city.IsActive,
                CreatedAt = city.CreatedAt
            };

            return Result<CityDto>.Created(dto, LocalizationKeys.City.Created);
        }
    }
}
