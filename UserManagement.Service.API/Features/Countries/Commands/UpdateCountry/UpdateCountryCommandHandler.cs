using MediatR;
using SNUL.Shared.Common.DTOs.UserManagement;
using SNUL.Shared.Common.Interfaces;
using SNUL.Shared.Common.Repositories.Interfaces.Base;
using SNUL.Shared.Domain.Models;
using SNUL.Shared.Localization;
using SNUL.Shared.Results;

namespace UserManagement.Service.API.Features.Countries.Commands.UpdateCountry
{
    public class UpdateCountryCommandHandler : IRequestHandler<UpdateCountryCommand, Result<CountryDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public UpdateCountryCommandHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result<CountryDto>> Handle(UpdateCountryCommand request, CancellationToken cancellationToken)
        {
            var countryRepo = _unitOfWork.GetRepository<Country, Guid>();
            var country = await countryRepo.GetByIdAsync(request.Id, cancellationToken);
            if (country == null || country.IsDeleted)
            {
                return Result<CountryDto>.NotFound(LocalizationKeys.Country.NotFound);
            }

            var currentUserId = _currentUserService.UserId != Guid.Empty
                ? _currentUserService.UserId.ToString()
                : "System";

            country.Update(request.NameEn, request.NameAr, request.Code, request.PhoneCode, currentUserId);

            if (request.IsActive.HasValue)
                country.SetActiveState(request.IsActive.Value, currentUserId);
            countryRepo.Update(country);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var dto = new CountryDto
            {
                Id = country.Id,
                NameEn = country.NameEn,
                NameAr = country.NameAr,
                Code = country.Code,
                PhoneCode = country.PhoneCode,
                IsActive = country.IsActive,
                CreatedAt = country.CreatedAt
            };

            return Result<CountryDto>.Success(dto, LocalizationKeys.Country.Updated);
        }
    }
}
