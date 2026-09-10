using MediatR;
using SNUL.Shared.Common.DTOs.UserManagement;
using SNUL.Shared.Common.Interfaces;
using SNUL.Shared.Common.Repositories.Interfaces.Base;
using SNUL.Shared.Domain.Models;
using SNUL.Shared.Localization;
using SNUL.Shared.Results;

namespace UserManagement.Service.API.Features.Countries.Commands.CreateCountry
{
    public class CreateCountryCommandHandler : IRequestHandler<CreateCountryCommand, Result<CountryDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public CreateCountryCommandHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result<CountryDto>> Handle(CreateCountryCommand request, CancellationToken cancellationToken)
        {
            var countryRepo = _unitOfWork.GetRepository<Country, Guid>();
            var exists = await countryRepo.ExistsAsync(
                c => (!c.IsDeleted) && (c.NameEn.ToLower() == request.NameEn.Trim().ToLower() || c.NameAr == request.NameAr.Trim()),
                cancellationToken);

            if (exists)
            {
                return Result<CountryDto>.Conflict(LocalizationKeys.Country.AlreadyExists);
            }

            var currentUserId = _currentUserService.UserId != Guid.Empty
                ? _currentUserService.UserId.ToString()
                : "System";

            var country = Country.Create(
                request.NameEn.Trim(),
                request.NameAr.Trim(),
                request.Code?.Trim(),
                request.PhoneCode?.Trim(),
                currentUserId);

            await countryRepo.AddAsync(country, cancellationToken);
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

            return Result<CountryDto>.Created(dto, LocalizationKeys.Country.Created);
        }
    }
}
