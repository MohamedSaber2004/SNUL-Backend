using MediatR;
using Microsoft.EntityFrameworkCore;
using SNUL.Shared.Common.DTOs.UserManagement;
using SNUL.Shared.Common.Repositories.Interfaces.Base;
using SNUL.Shared.Domain.Models;
using SNUL.Shared.Localization;
using SNUL.Shared.Results;

namespace UserManagement.Service.API.Features.DistributorApplications.Queries.GetDistributorApplicationById
{
    public class GetDistributorApplicationByIdQueryHandler : IRequestHandler<GetDistributorApplicationByIdQuery, Result<DistributorApplicationDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetDistributorApplicationByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<DistributorApplicationDto>> Handle(GetDistributorApplicationByIdQuery request, CancellationToken cancellationToken)
        {
            var repo = _unitOfWork.GetRepository<DistributorApplication, Guid>();
            var app = await repo.GetAll(a => a.Id == request.Id && !a.IsDeleted)
                .Include(a => a.Country)
                .FirstOrDefaultAsync(cancellationToken);

            if (app == null)
            {
                return Result<DistributorApplicationDto>.NotFound(LocalizationKeys.DistributorApplication.NotFound);
            }

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

            return Result<DistributorApplicationDto>.Success(dto, LocalizationKeys.DistributorApplication.Fetched);
        }
    }
}
