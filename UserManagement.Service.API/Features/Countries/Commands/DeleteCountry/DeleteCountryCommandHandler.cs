using MediatR;
using SNUL.Shared.Common.Interfaces;
using SNUL.Shared.Common.Repositories.Interfaces.Base;
using SNUL.Shared.Domain.Models;
using SNUL.Shared.Localization;
using SNUL.Shared.Results;

namespace UserManagement.Service.API.Features.Countries.Commands.DeleteCountry
{
    public class DeleteCountryCommandHandler : IRequestHandler<DeleteCountryCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public DeleteCountryCommandHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result<string>> Handle(DeleteCountryCommand request, CancellationToken cancellationToken)
        {
            var countryRepo = _unitOfWork.GetRepository<Country, Guid>();
            var country = await countryRepo.GetByIdAsync(request.Id, cancellationToken);
            if (country == null || country.IsDeleted)
            {
                return Result<string>.NotFound(LocalizationKeys.Country.NotFound);
            }

            var currentUserId = _currentUserService.UserId != Guid.Empty
                ? _currentUserService.UserId.ToString()
                : "System";

            country.MarkAsDeleted(currentUserId);
            countryRepo.Update(country);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<string>.Success(country.Id.ToString(), LocalizationKeys.Country.Deleted);
        }
    }
}
