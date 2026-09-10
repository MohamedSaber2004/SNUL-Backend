using MediatR;
using SNUL.Shared.Common.Interfaces;
using SNUL.Shared.Common.Repositories.Interfaces.Base;
using SNUL.Shared.Localization;
using SNUL.Shared.Results;
using CompanyEntity = SNUL.Shared.Domain.Models.Company;
namespace UserManagement.Service.API.Features.Companies.Commands.DeleteCompany
{
    public class DeleteCompanyCommandHandler : IRequestHandler<DeleteCompanyCommand, Result<string>>
    {
        private readonly IUnitOfWork _uow; private readonly ICurrentUserService _cur;
        public DeleteCompanyCommandHandler(IUnitOfWork uow, ICurrentUserService cur) { _uow = uow; _cur = cur; }
        public async Task<Result<string>> Handle(DeleteCompanyCommand r, CancellationToken ct)
        {
            var repo = _uow.GetRepository<CompanyEntity, Guid>();
            var c = await repo.GetByIdAsync(r.Id, ct);
            if (c == null || c.IsDeleted) return Result<string>.NotFound(LocalizationKeys.Company.NotFound);
            var curId = _cur.UserId != Guid.Empty ? _cur.UserId.ToString() : "System";
            c.MarkAsDeleted(curId); repo.Update(c); await _uow.SaveChangesAsync(ct);
            return Result<string>.Success(c.Id.ToString(), LocalizationKeys.Company.Deleted);
        }
    }
}
