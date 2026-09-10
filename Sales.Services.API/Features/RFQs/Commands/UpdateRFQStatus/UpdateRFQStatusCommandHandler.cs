using MediatR;
using SNUL.Shared.Common.Repositories.Interfaces.Base;
using SNUL.Shared.Localization;
using SNUL.Shared.Results;
using RFQEntity = SNUL.Shared.Domain.Models.RFQ;
namespace Sales.Services.API.Features.RFQs.Commands.UpdateRFQStatus
{
    public class UpdateRFQStatusCommandHandler : IRequestHandler<UpdateRFQStatusCommand, Result<string>>
    {
        private readonly IUnitOfWork _uow;
        public UpdateRFQStatusCommandHandler(IUnitOfWork uow) => _uow = uow;
        public async Task<Result<string>> Handle(UpdateRFQStatusCommand r, CancellationToken ct)
        {
            var repo = _uow.GetRepository<RFQEntity, Guid>();
            var rfq = await repo.GetByIdAsync(r.Id, ct);
            if (rfq == null || rfq.IsDeleted) return Result<string>.NotFound(LocalizationKeys.RFQ.NotFound);
            if (Enum.TryParse<SNUL.Shared.Domain.Models.RFQStatus>(r.Status, true, out var st)) rfq.Status = st;
            else return Result<string>.BadRequest(LocalizationKeys.RFQ.InvalidStatus);
            rfq.MarkAsUpdated("System"); repo.Update(rfq); await _uow.SaveChangesAsync(ct);
            return Result<string>.Success(rfq.Id.ToString(), LocalizationKeys.RFQ.Updated);
        }
    }
}
