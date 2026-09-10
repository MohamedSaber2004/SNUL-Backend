using MediatR;
using SNUL.Shared.Common.Interfaces;
using SNUL.Shared.Common.Repositories.Interfaces.Base;
using SNUL.Shared.Localization;
using SNUL.Shared.Results;
using CurrencyEntity = SNUL.Shared.Domain.Models.Currency;

namespace Product.Services.API.Features.Currencies.Commands.DeleteCurrency
{
    public class DeleteCurrencyCommandHandler : IRequestHandler<DeleteCurrencyCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public DeleteCurrencyCommandHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result<string>> Handle(DeleteCurrencyCommand request, CancellationToken cancellationToken)
        {
            var currencyRepo = _unitOfWork.GetRepository<CurrencyEntity, Guid>();
            var currency = await currencyRepo.GetByIdAsync(request.Id, cancellationToken);

            if (currency == null || currency.IsDeleted)
            {
                return Result<string>.NotFound(LocalizationKeys.Currency.NotFound);
            }

            var currentUserId = _currentUserService.UserId != Guid.Empty
                ? _currentUserService.UserId.ToString()
                : "System";

            currency.MarkAsDeleted(currentUserId);
            currencyRepo.Update(currency);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<string>.Success(currency.Id.ToString(), LocalizationKeys.Currency.Deleted);
        }
    }
}
