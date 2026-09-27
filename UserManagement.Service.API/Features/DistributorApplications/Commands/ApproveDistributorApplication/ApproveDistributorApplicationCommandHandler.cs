using MediatR;
using Microsoft.EntityFrameworkCore;
using SNUL.Shared.Common.DTOs.UserManagement;
using SNUL.Shared.Common.Interfaces;
using SNUL.Shared.Common.Repositories.Interfaces.Base;
using SNUL.Shared.Domain.Models;
using SNUL.Shared.Enums;
using SNUL.Shared.Localization;
using SNUL.Shared.Results;

namespace UserManagement.Service.API.Features.DistributorApplications.Commands.ApproveDistributorApplication
{
    public class ApproveDistributorApplicationCommandHandler : IRequestHandler<ApproveDistributorApplicationCommand, Result<DistributorApplicationDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public ApproveDistributorApplicationCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result<DistributorApplicationDto>> Handle(ApproveDistributorApplicationCommand request, CancellationToken cancellationToken)
        {
            var appRepo = _unitOfWork.GetRepository<DistributorApplication, Guid>();
            var app = await appRepo.GetAll(a => a.Id == request.Id && !a.IsDeleted)
                .Include(a => a.Country)
                .FirstOrDefaultAsync(cancellationToken);

            if (app == null)
            {
                return Result<DistributorApplicationDto>.NotFound(LocalizationKeys.DistributorApplication.NotFound);
            }

            var currentUserId = _currentUserService.UserId != Guid.Empty
                ? _currentUserService.UserId.ToString()
                : "System";

if (app.Status == DistributorApplicationStatus.Approved)
            {
                var healCompanyRepo = _unitOfWork.GetRepository<Company, Guid>();
                var approvedCompany = await healCompanyRepo.GetAll(c => c.Name.ToLower() == app.CompanyName.ToLower() && !c.IsDeleted)
                    .FirstOrDefaultAsync(cancellationToken);

                if (approvedCompany != null
                    && approvedCompany.Status == CompanyStatus.Approved
                    && await TryLinkApplicantAsync(app, approvedCompany.Id, currentUserId, cancellationToken))
                {
                    // Heal rows written before approval started setting these:
                    // re-approving an already-approved application is the only
                    // place they can be corrected.
                    if (!approvedCompany.IsProvider || !approvedCompany.IsActive)
                    {
                        approvedCompany.IsProvider = true;
                        approvedCompany.SetActiveState(true, currentUserId);
                    }
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    return Result<DistributorApplicationDto>.Success(ToDto(app), LocalizationKeys.DistributorApplication.Approved);
                }

                return Result<DistributorApplicationDto>.BadRequest(LocalizationKeys.DistributorApplication.AlreadyProcessed);
            }

            app.Status = DistributorApplicationStatus.Approved;
            app.MarkAsUpdated(currentUserId);

var companyRepo = _unitOfWork.GetRepository<Company, Guid>();
            var existingCompany = await companyRepo.GetAll(c => c.Name.ToLower() == app.CompanyName.ToLower() && !c.IsDeleted)
                .FirstOrDefaultAsync(cancellationToken);

            Guid companyId;
            if (existingCompany != null)
            {
                existingCompany.Update(
                    existingCompany.Name,
                    app.Type,
                    app.CountryId,
                    CompanyStatus.Approved,
                    request.AccountManagerId,
                    currentUserId,
                    string.IsNullOrWhiteSpace(existingCompany.Email) ? app.ContactEmail : existingCompany.Email);
                // Approval is what makes a company a provider: without these two
                // lines an approved applicant is invisible to the public provider
                // directory and to every provider-scoped query.
                existingCompany.IsProvider = true;
                existingCompany.SetActiveState(true, currentUserId);
                companyId = existingCompany.Id;
            }
            else
            {
                var newCompany = Company.Create(
                    app.CompanyName,
                    app.Type,
                    app.CountryId,
                    CompanyStatus.Approved,
                    request.AccountManagerId,
                    currentUserId,
                    app.ContactEmail);
                newCompany.IsProvider = true;
                newCompany.SetActiveState(true, currentUserId);
                await companyRepo.AddAsync(newCompany, cancellationToken);
                companyId = newCompany.Id;
            }

await TryLinkApplicantAsync(app, companyId, currentUserId, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<DistributorApplicationDto>.Success(ToDto(app), LocalizationKeys.DistributorApplication.Approved);
        }

                private async Task<bool> TryLinkApplicantAsync(DistributorApplication app, Guid companyId, string currentUserId, CancellationToken cancellationToken)
        {
            var userRepo = _unitOfWork.GetRepository<ApplicationUser, Guid>();
            ApplicationUser? applicant = null;

            if (!string.IsNullOrWhiteSpace(app.CreatedBy))
            {
                if (Guid.TryParse(app.CreatedBy, out var createdById) && createdById != Guid.Empty)
                {
                    applicant = await userRepo.GetByIdAsync(createdById, cancellationToken);
                }
                else
                {
                    var createdByEmail = app.CreatedBy.Trim().ToLower();
                    applicant = await userRepo.GetAll(u => !u.IsDeleted && (u.Email ?? "").ToLower() == createdByEmail)
                        .FirstOrDefaultAsync(cancellationToken);
                }
            }

            if ((applicant == null || applicant.IsDeleted) && !string.IsNullOrWhiteSpace(app.ContactEmail))
            {
                var email = app.ContactEmail.Trim().ToLower();
                applicant = await userRepo.GetAll(u => !u.IsDeleted && (u.Email ?? "").ToLower() == email)
                    .FirstOrDefaultAsync(cancellationToken);
            }

            if (applicant == null || applicant.IsDeleted)
                return false;

            // Promotion is what makes the link effective: ProviderScope (and the
            // Sales BuyerScope) key off UserType.OrganizationUser, so linking a
            // CompanyId alone would leave the applicant unable to act as a
            // provider until their next login happened to self-heal.
            var modified = false;
            if (applicant.CompanyId != companyId)
            {
                applicant.CompanyId = companyId;
                modified = true;
            }
            if (applicant.UserType != UserType.OrganizationUser)
            {
                applicant.UserType = UserType.OrganizationUser;
                modified = true;
            }
            if (!applicant.IsActive)
            {
                applicant.IsActive = true;
                modified = true;
            }
            if (!applicant.EmailConfirmed)
            {
                applicant.EmailConfirmed = true;
                modified = true;
            }
            if (modified)
                applicant.MarkAsUpdated(currentUserId);

            return modified;
        }

        private static DistributorApplicationDto ToDto(DistributorApplication app)
        {
            return new DistributorApplicationDto
            {
                Id = app.Id,
                CompanyName = app.CompanyName,
                Type = app.Type,
                CountryId = app.CountryId,
                CountryNameEn = app.Country != null ? app.Country.NameEn : null,
                SalesVolumeBand = app.SalesVolumeBand,
                Website = app.Website,
                ContactPerson = app.ContactPerson,
                ContactEmail = app.ContactEmail,
                Status = app.Status.ToString(),
                CreatedAt = app.CreatedAt,
                UpdatedAt = app.UpdatedAt
            };
        }
    }
}
