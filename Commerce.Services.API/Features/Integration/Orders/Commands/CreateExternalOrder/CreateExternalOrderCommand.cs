using MediatR;
using SNUL.Shared.Common.DTOs.Integration;
using SNUL.Shared.Common.Interfaces;
using SNUL.Shared.Common.Repositories.Interfaces.Base;
using SNUL.Shared.Domain.Models;
using SNUL.Shared.Results;

namespace Commerce.Services.API.Features.Integration.Orders.Commands.CreateExternalOrder
{
    public class CreateExternalOrderCommand : IRequest<Result<ExternalOrderResponse>>, IWelcoSystemRequest
    {
        public string? SourceMarket { get; set; }
        public Guid? ExternalCustomerId { get; set; }
        public Guid? CurrencyId { get; set; }
        public decimal TotalAmount { get; set; }
        public List<ExternalOrderItemRequest> Items { get; set; } = new();
        public string? System { get; set; }
    }

    public class CreateExternalOrderCommandHandler : IRequestHandler<CreateExternalOrderCommand, Result<ExternalOrderResponse>>
    {
        private readonly IWelcoIntegrationService _welcoService;
        private readonly IWelcoSystemResolver _resolver;
        private readonly ICurrentUserService _currentUser;
        private readonly IUnitOfWork _unitOfWork;

        public CreateExternalOrderCommandHandler(
            IWelcoIntegrationService welcoService,
            IWelcoSystemResolver resolver,
            ICurrentUserService currentUser,
            IUnitOfWork unitOfWork)
        {
            _welcoService = welcoService;
            _resolver = resolver;
            _currentUser = currentUser;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<ExternalOrderResponse>> Handle(CreateExternalOrderCommand request, CancellationToken ct)
        {
            // Isolation: a caller bound to a company may only order for that company's
            // mapped Welco customer. Admins (no company) are unrestricted.
            if (request.ExternalCustomerId.HasValue && _currentUser.IsAuthenticated && _currentUser.UserId != Guid.Empty)
            {
                var user = await _unitOfWork.GetRepository<ApplicationUser, Guid>()
                    .FindByKeyAsync(_currentUser.UserId, ct);
                if (user?.CompanyId.HasValue == true)
                {
                    var company = await _unitOfWork.GetRepository<Company, Guid>()
                        .FindByKeyAsync(user.CompanyId.Value, ct);
                    if (company is not null && company.WelcoCompanyId.HasValue &&
                        company.WelcoCompanyId.Value != request.ExternalCustomerId.Value)
                    {
                        return Result<ExternalOrderResponse>.Forbidden();
                    }
                }
            }

            var req = new CreateExternalOrderRequest
            {
                SourceMarket = string.IsNullOrWhiteSpace(request.SourceMarket)
                    ? _resolver.Resolve(request.System).Market
                    : request.SourceMarket,
                ExternalCustomerId = request.ExternalCustomerId,
                CurrencyId = request.CurrencyId,
                TotalAmount = request.TotalAmount,
                Items = request.Items
            };
            return await _welcoService.CreateOrderAsync(req, ct, request.System);
        }
    }
}
