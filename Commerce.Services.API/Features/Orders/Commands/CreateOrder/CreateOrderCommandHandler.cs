using Commerce.Services.API.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SNUL.Shared.Common.DTOs.Commerce;
using Microsoft.Extensions.Options;
using SNUL.Shared.Common.Interfaces;
using SNUL.Shared.Common.Options;
using SNUL.Shared.Common.Repositories.Interfaces.Base;
using SNUL.Shared.Localization;
using SNUL.Shared.Results;
using OrderEntity = SNUL.Shared.Domain.Models.Order;
using OrderItemEntity = SNUL.Shared.Domain.Models.OrderItem;
using ProductEntity = SNUL.Shared.Domain.Models.Product;
using CompanyEntity = SNUL.Shared.Domain.Models.Company;
using CurrencyEntity = SNUL.Shared.Domain.Models.Currency;
using ApplicationUser = SNUL.Shared.Domain.Models.ApplicationUser;

namespace Commerce.Services.API.Features.Orders.Commands.CreateOrder
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Result<OrderDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly ICurrentUserService _currentUser;
        private readonly IExchangeRateService _exchangeRateService;
        private readonly ExchangeRateSettings _fxSettings;

        public CreateOrderCommandHandler(IUnitOfWork uow, ICurrentUserService currentUser, IExchangeRateService exchangeRateService, IOptions<ExchangeRateSettings> fxOptions)
        {
            _uow = uow;
            _currentUser = currentUser;
            _exchangeRateService = exchangeRateService;
            _fxSettings = fxOptions.Value;
        }

        public async Task<Result<OrderDto>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            if (request.Items == null || !request.Items.Any())
                return Result<OrderDto>.BadRequest(LocalizationKeys.Order.ItemsRequired);

            if (request.CompanyId.HasValue)
            {
                var companyRepo = _uow.GetRepository<CompanyEntity, Guid>();
                var exists = await companyRepo.ExistsAsync(c => !c.IsDeleted && c.Id == request.CompanyId.Value, cancellationToken);
                if (!exists) return Result<OrderDto>.NotFound(LocalizationKeys.Company.NotFound);
            }

            if (request.CurrencyId.HasValue)
            {
                var currencyRepo = _uow.GetRepository<CurrencyEntity, Guid>();
                var exists = await currencyRepo.ExistsAsync(c => !c.IsDeleted && c.Id == request.CurrencyId.Value, cancellationToken);
                if (!exists) return Result<OrderDto>.BadRequest(LocalizationKeys.Currency.NotFound);
            }
            else if (!string.IsNullOrWhiteSpace(request.CurrencyCode))
            {
                var currencyRepo = _uow.GetRepository<CurrencyEntity, Guid>();
                var code = request.CurrencyCode.Trim().ToUpperInvariant();
                var currency = await currencyRepo.GetAll(c => !c.IsDeleted && c.Code.ToUpper() == code).FirstOrDefaultAsync(cancellationToken);
                if (currency == null) return Result<OrderDto>.BadRequest(LocalizationKeys.Currency.NotFound);
                request.CurrencyId = currency.Id;
            }

            var productRepo = _uow.GetRepository<ProductEntity, Guid>();
            foreach (var it in request.Items)
            {
                var exists = await productRepo.ExistsAsync(p => !p.IsDeleted && p.Id == it.ProductId, cancellationToken);
                if (!exists) return Result<OrderDto>.NotFound(LocalizationKeys.Product.NotFound);
            }

            var currentUserId = _currentUser.UserId != Guid.Empty ? _currentUser.UserId.ToString() : "System";

            string? snapshotBase = null;
            string? snapshotCode = null;
            decimal? snapshotRate = null;
            DateOnly? snapshotDate = null;
            string? snapshotSource = null;
            if (request.CurrencyId.HasValue)
            {
                try
                {
                    var currencyRepo = _uow.GetRepository<CurrencyEntity, Guid>();
                    var cur = await currencyRepo.GetByIdAsync(request.CurrencyId.Value, cancellationToken);
                    if (cur != null)
                    {
                        snapshotCode = cur.Code;
                        snapshotBase = string.IsNullOrWhiteSpace(_fxSettings.BaseCurrency) ? "USD" : _fxSettings.BaseCurrency.Trim().ToUpperInvariant();
                        if (snapshotCode != snapshotBase)
                        {
                            var conv = await _exchangeRateService.ConvertWithDetailsAsync(1m, snapshotBase, snapshotCode, cancellationToken);
                            snapshotRate = conv.Rate;
                            snapshotDate = conv.RateDate;
                            snapshotSource = conv.Source;
                        }
                        else
                        {
                            snapshotRate = 1m;
                            snapshotDate = DateOnly.FromDateTime(DateTime.UtcNow.Date);
                            snapshotSource = "identity";
                        }
                    }
                }
                catch (Exception)
                {
                }
            }

            var effectiveUserId = request.UserId ?? (_currentUser.UserId != Guid.Empty ? _currentUser.UserId : (Guid?)null);
            var effectiveCompanyId = request.CompanyId;
            if (!effectiveCompanyId.HasValue && effectiveUserId.HasValue)
            {
                var userRepo = _uow.GetRepository<ApplicationUser, Guid>();
                var u = await userRepo.GetByIdAsync(effectiveUserId.Value, cancellationToken);
                if (u?.CompanyId.HasValue == true)
                {
                    effectiveCompanyId = u.CompanyId.Value;
                }
            }

            var order = new OrderEntity
            {
                Id = Guid.NewGuid(),
                OrderNumber = $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}",
                Status = SNUL.Shared.Domain.Models.OrderStatus.Pending,
                UserId = effectiveUserId,
                CompanyId = effectiveCompanyId,
                CurrencyId = request.CurrencyId,
                QuoteId = request.QuoteId,
                TotalAmount = request.Items.Sum(i => i.Quantity * i.UnitPrice),
                SnapshotBaseCurrency = snapshotBase,
                SnapshotCurrencyCode = snapshotCode,
                SnapshotRate = snapshotRate,
                SnapshotRateDate = snapshotDate,
                SnapshotSource = snapshotSource
            };
            order.MarkAsCreated(currentUserId);

            foreach (var it in request.Items)
            {
                var orderItem = new OrderItemEntity
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    ProductId = it.ProductId,
                    Quantity = it.Quantity,
                    UnitPrice = it.UnitPrice
                };
                orderItem.MarkAsCreated(currentUserId);
                order.Items.Add(orderItem);
            }

            var repo = _uow.GetRepository<OrderEntity, Guid>();
            await repo.AddAsync(order, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            var saved = await repo.GetAll(o => !o.IsDeleted && o.Id == order.Id)
                .Include(o => o.Items)
                .FirstOrDefaultAsync(cancellationToken);

            var dto = CommerceDtoMapper.ToDto(saved!);
            return Result<OrderDto>.Created(dto, LocalizationKeys.Order.Created);
        }
    }
}
