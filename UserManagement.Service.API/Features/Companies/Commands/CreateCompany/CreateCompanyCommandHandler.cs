using MediatR;
using SNUL.Shared.Common.DTOs.UserManagement;
using SNUL.Shared.Common.Interfaces;
using SNUL.Shared.Common.Repositories.Interfaces.Base;
using SNUL.Shared.Localization;
using SNUL.Shared.Results;
using CompanyEntity = SNUL.Shared.Domain.Models.Company;
namespace UserManagement.Service.API.Features.Companies.Commands.CreateCompany
{
    public class CreateCompanyCommandHandler : IRequestHandler<CreateCompanyCommand, Result<CompanyDto>>
    {
        private readonly IUnitOfWork _uow; private readonly ICurrentUserService _cur;
        public CreateCompanyCommandHandler(IUnitOfWork uow, ICurrentUserService cur) { _uow = uow; _cur = cur; }
        public async Task<Result<CompanyDto>> Handle(CreateCompanyCommand r, CancellationToken ct)
        {
            var repo = _uow.GetRepository<CompanyEntity, Guid>();
            var countryRepo = _uow.GetRepository<SNUL.Shared.Domain.Models.Country, Guid>();
            if (!await countryRepo.ExistsAsync(c => !c.IsDeleted && c.Id == r.CountryId, ct)) return Result<CompanyDto>.BadRequest(LocalizationKeys.Company.CountryRequired);
            var curId = _cur.UserId != Guid.Empty ? _cur.UserId.ToString() : "System";
            var c = CompanyEntity.Create(r.Name.Trim(), r.Type, r.CountryId, r.Status, r.AccountManagerId, curId, r.Email, r.ImageName);
            c.IsProvider = true;
            await repo.AddAsync(c, ct); await _uow.SaveChangesAsync(ct);
            return Result<CompanyDto>.Created(new CompanyDto { Id = c.Id, Name = c.Name, Email = c.Email, ImageName = c.ImageName, Type = c.Type, CountryId = c.CountryId, Status = c.Status, AccountManagerId = c.AccountManagerId, IsActive = c.IsActive, IsProvider = c.IsProvider, CreatedAt = c.CreatedAt }, LocalizationKeys.Company.Created);
        }
    }
}
