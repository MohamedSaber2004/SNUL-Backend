using MediatR;
using Microsoft.EntityFrameworkCore;
using SNUL.Shared.Common.DTOs.UserManagement;
using SNUL.Shared.Common.Interfaces;
using SNUL.Shared.Common.Repositories.Interfaces.Base;
using SNUL.Shared.Domain.Models;
using SNUL.Shared.Localization;
using SNUL.Shared.Results;

namespace UserManagement.Service.API.Features.DistributorApplications.Commands.RejectDistributorApplication
{
    public class RejectDistributorApplicationCommandHandler : IRequestHandler<RejectDistributorApplicationCommand, Result<DistributorApplicationDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public RejectDistributorApplicationCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result<DistributorApplicationDto>> Handle(RejectDistributorApplicationCommand request, CancellationToken cancellationToken)
        {
            var appRepo = _unitOfWork.GetRepository<DistributorApplication, Guid>();
            var app = await appRepo.GetAll(a => a.Id == request.Id && !a.IsDeleted)
                .Include(a => a.Country)
                .FirstOrDefaultAsync(cancellationToken);

            if (app == null)
            {
                return Result<DistributorApplicationDto>.NotFound(LocalizationKeys.DistributorApplication.NotFound);
            }

            if (app.Status == DistributorApplicationStatus.Rejected)
            {
                return Result<DistributorApplicationDto>.BadRequest(LocalizationKeys.DistributorApplication.AlreadyProcessed);
            }

            var currentUserId = _currentUserService.UserId != Guid.Empty
                ? _currentUserService.UserId.ToString()
                : "System";

            app.Status = DistributorApplicationStatus.Rejected;
            app.MarkAsUpdated(currentUserId);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var dto = new DistributorApplicationDto
            {
                Id = app.Id,
                CompanyName = app.CompanyName,
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

            return Result<DistributorApplicationDto>.Success(dto, LocalizationKeys.DistributorApplication.Rejected);
        }
    }
}
