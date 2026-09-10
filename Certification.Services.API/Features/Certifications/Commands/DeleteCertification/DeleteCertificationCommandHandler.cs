using MediatR;
using SNUL.Shared.Common.Interfaces;
using SNUL.Shared.Common.Repositories.Interfaces.Base;
using SNUL.Shared.Localization;
using SNUL.Shared.Results;
using CertificationEntity = SNUL.Shared.Domain.Models.Certification;

namespace Certification.Services.API.Features.Certifications.Commands.DeleteCertification
{
    public class DeleteCertificationCommandHandler : IRequestHandler<DeleteCertificationCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public DeleteCertificationCommandHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result<string>> Handle(DeleteCertificationCommand request, CancellationToken cancellationToken)
        {
            var certificationRepo = _unitOfWork.GetRepository<CertificationEntity, Guid>();
            var certification = await certificationRepo.GetByIdAsync(request.Id, cancellationToken);

            if (certification == null || certification.IsDeleted)
            {
                return Result<string>.NotFound(LocalizationKeys.Certification.NotFound);
            }

            var currentUserId = _currentUserService.UserId != Guid.Empty
                ? _currentUserService.UserId.ToString()
                : "System";

            certification.MarkAsDeleted(currentUserId);
            certificationRepo.Update(certification);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<string>.Success(certification.Id.ToString(), LocalizationKeys.Certification.Deleted);
        }
    }
}
