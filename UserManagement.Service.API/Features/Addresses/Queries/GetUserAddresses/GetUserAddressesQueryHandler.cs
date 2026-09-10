using MediatR;
using Microsoft.EntityFrameworkCore;
using SNUL.Shared.Common.DTOs.UserManagement;
using SNUL.Shared.Common.Repositories.Interfaces.Base;
using SNUL.Shared.Domain.Models;
using SNUL.Shared.Localization;
using SNUL.Shared.Results;

namespace UserManagement.Service.API.Features.Addresses.Queries.GetUserAddresses
{
    public class GetUserAddressesQueryHandler : IRequestHandler<GetUserAddressesQuery, Result<IReadOnlyList<UserAddressDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetUserAddressesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<IReadOnlyList<UserAddressDto>>> Handle(GetUserAddressesQuery request, CancellationToken cancellationToken)
        {
            var addressRepo = _unitOfWork.GetRepository<UserAddress, Guid>();
            var addresses = await addressRepo
                .GetAllWithIncluding(a => a.UserId == request.UserId && !a.IsDeleted, a => a.Country, a => a.City, a => a.Zone)
                .OrderByDescending(a => a.IsDefault)
                .ThenByDescending(a => a.CreatedAt)
                .Select(a => new UserAddressDto
                {
                    Id = a.Id,
                    UserId = a.UserId,
                    CountryId = a.CountryId,
                    CountryNameEn = a.Country != null ? a.Country.NameEn : null,
                    CountryNameAr = a.Country != null ? a.Country.NameAr : null,
                    CountryCode = a.Country != null ? a.Country.Code : null,
                    CountryPhoneCode = a.Country != null ? a.Country.PhoneCode : null,
                    CityId = a.CityId,
                    CityNameEn = a.City != null ? a.City.NameEn : null,
                    CityNameAr = a.City != null ? a.City.NameAr : null,
                    ZoneId = a.ZoneId,
                    ZoneNameEn = a.Zone != null ? a.Zone.NameEn : null,
                    ZoneNameAr = a.Zone != null ? a.Zone.NameAr : null,
                    Street = a.Street,
                    Building = a.Building,
                    Floor = a.Floor,
                    Apartment = a.Apartment,
                    IsDefault = a.IsDefault,
                    CreatedAt = a.CreatedAt,
                    UpdatedAt = a.UpdatedAt
                })
                .ToListAsync(cancellationToken);

            return Result<IReadOnlyList<UserAddressDto>>.Success(addresses, LocalizationKeys.UserAddress.AddressesFetched);
        }
    }
}
